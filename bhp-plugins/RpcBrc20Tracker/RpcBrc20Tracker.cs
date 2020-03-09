using Microsoft.AspNetCore.Http;
using Bhp.IO.Caching;
using Bhp.IO.Data.LevelDB;
using Bhp.IO.Json;
using Bhp.Ledger;
using Bhp.Network.P2P.Payloads;
using Bhp.Network.RPC;
using Bhp.Persistence;
using Bhp.Persistence.LevelDB;
using Bhp.SmartContract;
using Bhp.VM;
using Bhp.Wallets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Snapshot = Bhp.Persistence.Snapshot;

namespace Bhp.Plugins
{
    public class RpcBrc20Tracker : Plugin, IPersistencePlugin, IRpcPlugin
    {
        private const byte Brc20BalancePrefix = 0xf8;
        private const byte Brc20TransferSentPrefix = 0xf9;
        private const byte Brc20TransferReceivedPrefix = 0xfa;
        private DB _db;
        private DataCache<Brc20BalanceKey, Brc20Balance> _balances;
        private DataCache<Brc20TransferKey, Brc20Transfer> _transfersSent;
        private DataCache<Brc20TransferKey, Brc20Transfer> _transfersReceived;
        private WriteBatch _writeBatch;
        private bool _shouldTrackHistory;
        private bool _recordNullAddressHistory;
        private uint _maxResults;
        private bool _shouldTrackNonStandardMintTokensEvent;
        private Bhp.IO.Data.LevelDB.Snapshot _levelDbSnapshot;
        private static readonly Fixed8 maxGas = Fixed8.FromDecimal(1m);

        public override void Configure()
        {
            if (_db == null)
            {
                var dbPath = GetConfiguration().GetSection("DBPath").Value ?? "Brc20BalanceData";
                _db = DB.Open(Path.GetFullPath(dbPath), new Options { CreateIfMissing = true });
            }
            _shouldTrackHistory = (GetConfiguration().GetSection("TrackHistory").Value ?? true.ToString()) != false.ToString();
            _recordNullAddressHistory = (GetConfiguration().GetSection("RecordNullAddressHistory").Value ?? false.ToString()) != false.ToString();
            _maxResults = uint.Parse(GetConfiguration().GetSection("MaxResults").Value ?? "1000");
            _shouldTrackNonStandardMintTokensEvent = (GetConfiguration().GetSection("TrackNonStandardMintTokens").Value ?? false.ToString()) != false.ToString();
        }

        private void ResetBatch()
        {
            _writeBatch = new WriteBatch();
            _levelDbSnapshot?.Dispose();
            _levelDbSnapshot = _db.GetSnapshot();
            ReadOptions dbOptions = new ReadOptions { FillCache = false, Snapshot = _levelDbSnapshot };
            _balances = new DbCache<Brc20BalanceKey, Brc20Balance>(_db, dbOptions, _writeBatch, Brc20BalancePrefix);
            if (_shouldTrackHistory)
            {
                _transfersSent =
                    new DbCache<Brc20TransferKey, Brc20Transfer>(_db, dbOptions, _writeBatch, Brc20TransferSentPrefix);
                _transfersReceived =
                    new DbCache<Brc20TransferKey, Brc20Transfer>(_db, dbOptions, _writeBatch, Brc20TransferReceivedPrefix);
            }
        }

        private void RecordTransferHistory(Snapshot snapshot, UInt160 scriptHash, UInt160 from, UInt160 to, BigInteger amount, UInt256 txHash, ref ushort transferIndex)
        {
            if (!_shouldTrackHistory) return;
            Header header = snapshot.GetHeader(snapshot.Height);
            if (_recordNullAddressHistory || from != UInt160.Zero)
            {
                _transfersSent.Add(new Brc20TransferKey(from, header.Timestamp, scriptHash, transferIndex),
                    new Brc20Transfer
                    {
                        Amount = amount,
                        UserScriptHash = to,
                        BlockIndex = snapshot.Height,
                        TxHash = txHash
                    });
            }

            if (_recordNullAddressHistory || to != UInt160.Zero)
            {
                _transfersReceived.Add(new Brc20TransferKey(to, header.Timestamp, scriptHash, transferIndex),
                    new Brc20Transfer
                    {
                        Amount = amount,
                        UserScriptHash = from,
                        BlockIndex = snapshot.Height,
                        TxHash = txHash
                    });
            }
            transferIndex++;
        }

        private void HandleNotification(Snapshot snapshot, Transaction transaction, UInt160 scriptHash,
            VM.Types.Array stateItems,
            Dictionary<Brc20BalanceKey, Brc20Balance> brc20BalancesChanged, ref ushort transferIndex)
        {
            if (stateItems.Count == 0) return;
            // Event name should be encoded as a byte array.
            if (!(stateItems[0] is VM.Types.ByteArray)) return;
            var eventName = Encoding.UTF8.GetString(stateItems[0].GetByteArray());

            if (_shouldTrackNonStandardMintTokensEvent && eventName == "mintTokens")
            {
                if (stateItems.Count < 4) return;
                // This is not an official standard but at least one token uses it, and so it is needed for proper
                // balance tracking to support all tokens in use.
                if (!(stateItems[2] is VM.Types.ByteArray))
                    return;
                byte[] mintToBytes = stateItems[2].GetByteArray();
                if (mintToBytes.Length != 20) return;
                var mintTo = new UInt160(mintToBytes);

                var mintAmountItem = stateItems[3];
                if (!(mintAmountItem is VM.Types.ByteArray || mintAmountItem is VM.Types.Integer))
                    return;

                var toKey = new Brc20BalanceKey(mintTo, scriptHash);
                if (!brc20BalancesChanged.ContainsKey(toKey)) brc20BalancesChanged.Add(toKey, new Brc20Balance());
                RecordTransferHistory(snapshot, scriptHash, UInt160.Zero, mintTo, mintAmountItem.GetBigInteger(), transaction.Hash, ref transferIndex);
                return;
            }
            if (eventName != "transfer") return;
            if (stateItems.Count < 4) return;

            if (!(stateItems[1] is null) && !(stateItems[1] is VM.Types.ByteArray))
                return;
            if (!(stateItems[2] is null) && !(stateItems[2] is VM.Types.ByteArray))
                return;
            var amountItem = stateItems[3];
            if (!(amountItem is VM.Types.ByteArray || amountItem is VM.Types.Integer))
                return;
            byte[] fromBytes = stateItems[1]?.GetByteArray();
            if (fromBytes?.Length != 20) fromBytes = null;
            byte[] toBytes = stateItems[2]?.GetByteArray();
            if (toBytes?.Length != 20) toBytes = null;
            if (fromBytes == null && toBytes == null) return;
            var from = new UInt160(fromBytes);
            var to = new UInt160(toBytes);

            if (fromBytes != null)
            {
                var fromKey = new Brc20BalanceKey(from, scriptHash);
                if (!brc20BalancesChanged.ContainsKey(fromKey)) brc20BalancesChanged.Add(fromKey, new Brc20Balance());
            }

            if (toBytes != null)
            {
                var toKey = new Brc20BalanceKey(to, scriptHash);
                if (!brc20BalancesChanged.ContainsKey(toKey)) brc20BalancesChanged.Add(toKey, new Brc20Balance());
            }
            RecordTransferHistory(snapshot, scriptHash, from, to, amountItem.GetBigInteger(), transaction.Hash, ref transferIndex);
        }

        public void OnPersist(Snapshot snapshot, IReadOnlyList<Blockchain.ApplicationExecuted> applicationExecutedList)
        {
            // Start freshly with a new DBCache for each block.
            ResetBatch();
            Dictionary<Brc20BalanceKey, Brc20Balance> brc20BalancesChanged = new Dictionary<Brc20BalanceKey, Brc20Balance>();

            ushort transferIndex = 0;
            foreach (Blockchain.ApplicationExecuted appExecuted in applicationExecutedList)
            {
                foreach (var executionResults in appExecuted.ExecutionResults)
                {
                    // Executions that fault won't modify storage, so we can skip them.
                    if (executionResults.VMState.HasFlag(VMState.FAULT)) continue;
                    foreach (var notifyEventArgs in executionResults.Notifications)
                    {
                        if (!(notifyEventArgs?.State is VM.Types.Array stateItems) || stateItems.Count == 0
                            || !(notifyEventArgs.ScriptContainer is Transaction transaction))
                            continue;
                        HandleNotification(snapshot, transaction, notifyEventArgs.ScriptHash, stateItems,
                            brc20BalancesChanged, ref transferIndex);
                    }
                }
            }

            foreach (var brc20BalancePair in brc20BalancesChanged)
            {
                // get guarantee accurate balances by calling balanceOf for keys that changed.
                byte[] script;
                using (ScriptBuilder sb = new ScriptBuilder())
                {
                    sb.EmitAppCall(brc20BalancePair.Key.AssetScriptHash, "balanceOf",
                        brc20BalancePair.Key.UserScriptHash.ToArray());
                    script = sb.ToArray();
                }

                using (ApplicationEngine engine = ApplicationEngine.Run(script, snapshot, extraGAS: maxGas))
                {
                    if (engine.State.HasFlag(VMState.FAULT)) continue;
                    if (engine.ResultStack.Count <= 0) continue;
                    brc20BalancePair.Value.Balance = engine.ResultStack.Pop().GetBigInteger();
                }

                brc20BalancePair.Value.LastUpdatedBlock = snapshot.Height;
                if (brc20BalancePair.Value.Balance == 0)
                {
                    _balances.Delete(brc20BalancePair.Key);
                    continue;
                }
                var itemToChange = _balances.GetAndChange(brc20BalancePair.Key, () => brc20BalancePair.Value);
                if (itemToChange != brc20BalancePair.Value)
                    itemToChange.FromReplica(brc20BalancePair.Value);
            }
        }

        public void OnCommit(Snapshot snapshot)
        {
            _balances.Commit();
            if (_shouldTrackHistory)
            {
                _transfersSent.Commit();
                _transfersReceived.Commit();
            }

            _db.Write(WriteOptions.Default, _writeBatch);
        }

        public bool ShouldThrowExceptionFromCommit(Exception ex)
        {
            return true;
        }

        private void AddTransfers(byte dbPrefix, UInt160 userScriptHash, uint startTime, uint endTime,
            JArray parentJArray)
        {
            var prefix = new[] { dbPrefix }.Concat(userScriptHash.ToArray()).ToArray();
            var startTimeBytes = BitConverter.GetBytes(startTime);
            var endTimeBytes = BitConverter.GetBytes(endTime);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(startTimeBytes);
                Array.Reverse(endTimeBytes);
            }

            var transferPairs = _db.FindRange<Brc20TransferKey, Brc20Transfer>(
                prefix.Concat(startTimeBytes).ToArray(),
                prefix.Concat(endTimeBytes).ToArray());

            int resultCount = 0;
            foreach (var transferPair in transferPairs)
            {
                if (++resultCount > _maxResults) break;
                JObject transfer = new JObject();
                transfer["timestamp"] = transferPair.Key.Timestamp;
                transfer["asset_hash"] = transferPair.Key.AssetScriptHash.ToArray().Reverse().ToHexString();
                transfer["transfer_address"] = transferPair.Value.UserScriptHash.ToAddress();
                transfer["amount"] = transferPair.Value.Amount.ToString();
                transfer["block_index"] = transferPair.Value.BlockIndex;
                transfer["transfer_notify_index"] = transferPair.Key.BlockXferNotificationIndex;
                transfer["tx_hash"] = transferPair.Value.TxHash.ToArray().Reverse().ToHexString();
                parentJArray.Add(transfer);
            }
        }

        private UInt160 GetScriptHashFromParam(string addressOrScriptHash)
        {
            return addressOrScriptHash.Length < 40 ?
                addressOrScriptHash.ToScriptHash() : UInt160.Parse(addressOrScriptHash);
        }
        private JObject GetBrc20Transfers(JArray _params)
        {
            UInt160 userScriptHash = GetScriptHashFromParam(_params[0].AsString());
            // If start time not present, default to 1 week of history.
            uint startTime = _params.Count > 1 ? (uint)_params[1].AsNumber() :
                (DateTime.UtcNow - TimeSpan.FromDays(7)).ToTimestamp();
            uint endTime = _params.Count > 2 ? (uint)_params[2].AsNumber() : DateTime.UtcNow.ToTimestamp();

            if (endTime < startTime) throw new RpcException(-32602, "Invalid params");

            JObject json = new JObject();
            JArray transfersSent = new JArray();
            json["sent"] = transfersSent;
            JArray transfersReceived = new JArray();
            json["received"] = transfersReceived;
            json["address"] = userScriptHash.ToAddress();
            AddTransfers(Brc20TransferSentPrefix, userScriptHash, startTime, endTime, transfersSent);
            AddTransfers(Brc20TransferReceivedPrefix, userScriptHash, startTime, endTime, transfersReceived);
            return json;
        }

        private JObject GetBrc20Balances(JArray _params)
        {
            UInt160 userScriptHash = GetScriptHashFromParam(_params[0].AsString());

            JObject json = new JObject();
            JArray balances = new JArray();
            json["balance"] = balances;
            json["address"] = userScriptHash.ToAddress();
            var dbCache = new DbCache<Brc20BalanceKey, Brc20Balance>(_db, null, null, Brc20BalancePrefix);
            byte[] prefix = userScriptHash.ToArray();
            foreach (var storageKeyValuePair in dbCache.Find(prefix))
            {
                JObject balance = new JObject();
                balance["asset_hash"] = storageKeyValuePair.Key.AssetScriptHash.ToArray().Reverse().ToHexString();
                balance["amount"] = storageKeyValuePair.Value.Balance.ToString();
                balance["last_updated_block"] = storageKeyValuePair.Value.LastUpdatedBlock;
                balances.Add(balance);
            }
            return json;
        }

        public void PreProcess(HttpContext context, string method, JArray _params)
        {
        }

        public JObject OnProcess(HttpContext context, string method, JArray _params)
        {
            if (_shouldTrackHistory && method == "getbrc20transfers") return GetBrc20Transfers(_params);
            return method == "getbrc20balances" ? GetBrc20Balances(_params) : null;
        }

        public void PostProcess(HttpContext context, string method, JArray _params, JObject result)
        {
        }
    }
}
