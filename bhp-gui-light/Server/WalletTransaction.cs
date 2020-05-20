using Bhp.BhpExtensions;
using Bhp.BhpExtensions.Fees;
using Bhp.Ledger;
using Bhp.Network.P2P.Payloads;
using Bhp.VM;
using Bhp.Wallets;
using System.Collections.Generic;
using System.Linq;

namespace Bhp.Server
{
    public class WalletTransaction
    {
        public T MakeTransaction<T>(Wallet wallet, T tx, out List<string> inputAddress, UInt160 from = null, UInt160 fee_address = null, UInt160 change_address = null, Fixed8 fee = default(Fixed8), Fixed8 transaction_fee = default(Fixed8)) where T : Transaction
        {
            inputAddress = new List<string>();
            if (tx.Outputs == null) tx.Outputs = new TransactionOutput[0];
            if (tx.Attributes == null) tx.Attributes = new TransactionAttribute[0];
            fee += tx.SystemFee;
            var pay_total = (typeof(T) == typeof(IssueTransaction) ? new TransactionOutput[0] : tx.Outputs).GroupBy(p => p.AssetId, (k, g) => new
            {
                AssetId = k,
                Value = g.Sum(p => p.Value)
            }).ToDictionary(p => p.AssetId);
            if (fee > Fixed8.Zero)
            {
                if (pay_total.ContainsKey(Blockchain.UtilityToken.Hash))
                {
                    pay_total[Blockchain.UtilityToken.Hash] = new
                    {
                        AssetId = Blockchain.UtilityToken.Hash,
                        Value = pay_total[Blockchain.UtilityToken.Hash].Value + fee
                    };
                }
                else
                {
                    pay_total.Add(Blockchain.UtilityToken.Hash, new
                    {
                        AssetId = Blockchain.UtilityToken.Hash,
                        Value = fee
                    });
                }
            }

            //By BHP
            //When transferring money, finding UTXO requires additional transaction fees
            bool hasBhpFeeAddress = false;
            if (pay_total.Any(p => p.Key.Equals(Blockchain.GoverningToken.Hash) && fee_address != null))
            {
                hasBhpFeeAddress = true;
            }
            var pay_coins = pay_total.Select(p => new
            {
                AssetId = p.Key,
                Unspents = from == null ? FindUnspentCoins(wallet, p.Key, p.Value.Value + BhpTxFee.EstimateTxFee(tx, p.Key, hasBhpFeeAddress)) :
                                          FindUnspentCoins(wallet, p.Key, p.Value.Value + BhpTxFee.EstimateTxFee(tx, p.Key, hasBhpFeeAddress), from)
            }).ToDictionary(p => p.AssetId);

            if (pay_coins.Any(p => p.Value.Unspents == null)) return null;
            var input_sum = pay_coins.Values.ToDictionary(p => p.AssetId, p => new
            {
                p.AssetId,
                Value = p.Unspents.Sum(q => q.Output.Value)
            });
            if (change_address == null) change_address = wallet.GetChangeAddress();
            List<TransactionOutput> outputs_new = new List<TransactionOutput>(tx.Outputs);

            List<int> changeNum = new List<int>();
            //添加找零地址  By BHP        
            foreach (UInt256 asset_id in input_sum.Keys)
            {
                Fixed8 txFee = BhpTxFee.EstimateTxFee(tx, asset_id, hasBhpFeeAddress);
                if (input_sum[asset_id].Value > (pay_total[asset_id].Value + txFee))
                {
                    outputs_new.Add(new TransactionOutput
                    {
                        AssetId = asset_id,
                        Value = input_sum[asset_id].Value - pay_total[asset_id].Value - txFee,
                        ScriptHash = change_address
                    });
                    changeNum.Add(outputs_new.Count - 1);
                }
            }

            //By BHP
            for (int i = 0; i < tx.Attributes.Length; i++)
            {
                if (tx.Attributes[i].Usage == TransactionAttributeUsage.SmartContractScript)
                {
                    using (ScriptBuilder sb = new ScriptBuilder())
                    {
                        sb.EmitPush(changeNum.Count);
                        if (changeNum.Count > 0)
                        {
                            for (int j = changeNum.Count - 1; j >= 0; j--)
                                sb.EmitPush(changeNum[j]);
                        }
                        sb.EmitPush(tx.Attributes[i].Data);
                        tx.Attributes[i].Data = sb.ToArray();
                    }
                }
            }

            tx.Inputs = pay_coins.Values.SelectMany(p => p.Unspents).Select(p => p.Reference).ToArray();
            tx.Outputs = outputs_new.ToArray();
            inputAddress = pay_coins.Values.SelectMany(p => p.Unspents).Select(p => p.Address).ToList();

            List<string> addr;
            tx = EstimateFee(wallet, tx, from, fee_address, out addr, hasBhpFeeAddress);//BHP            

            inputAddress.AddRange(addr);
            inputAddress = inputAddress.Distinct().ToList();
            return tx;
        }

        private T EstimateFee<T>(Wallet wallet, T tx, UInt160 from, UInt160 fee_address, out List<string> addr, bool hasBhpFeeAddress = false) where T : Transaction
        {
            addr = new List<string>();
            Fixed8 bhp_fee, coin_value;
            Coin[] feeCoins;

            if (hasBhpFeeAddress)
            {
                bhp_fee = BhpTxFee.EstimateTxFee(tx);
                feeCoins = FindUnspentCoins(wallet, Blockchain.GoverningToken.Hash, bhp_fee, fee_address);
                if (feeCoins == null) return null;
                tx.Inputs = tx.Inputs.Concat(feeCoins.Select(p => { return p.Reference; })).ToArray();
                coin_value = feeCoins.Sum(p => p.Output.Value);
                if (coin_value > bhp_fee)
                {
                    tx.Outputs = tx.Outputs.Concat(new[] { new TransactionOutput()
                        {
                            AssetId = Blockchain.GoverningToken.Hash,
                            ScriptHash = fee_address,
                            Value = coin_value - bhp_fee
                        }}).ToArray();
                }
                addr = feeCoins.Select(p => p.Address).ToList();
                return tx;
            }

            if (!ExtensionSettings.Default.WalletConfig.IsBhpFee) return tx;
            if (!tx.Outputs.Any(p => p.AssetId == Blockchain.GoverningToken.Hash))//without bhp
            {
                if (tx.Outputs.Any(p => p.AssetId != Blockchain.GoverningToken.Hash && p.AssetId != Blockchain.UtilityToken.Hash))//except bhp and gas
                {
                    bhp_fee = BhpTxFee.EstimateTxFee(tx);
                    if (fee_address == null && from != null)
                    {
                        fee_address = from;
                    }
                    feeCoins = fee_address == null ? FindUnspentCoins(wallet, Blockchain.GoverningToken.Hash, bhp_fee) :
                                                            FindUnspentCoins(wallet, Blockchain.GoverningToken.Hash, bhp_fee, fee_address);
                    if (feeCoins == null) return null;
                    tx.Inputs = tx.Inputs.Concat(feeCoins.Select(p => { return p.Reference; })).ToArray();
                    coin_value = feeCoins.Sum(p => p.Output.Value);
                    if (coin_value > bhp_fee)
                    {
                        tx.Outputs = tx.Outputs.Concat(new[] { new TransactionOutput()
                        {
                            AssetId = Blockchain.GoverningToken.Hash,
                            ScriptHash = fee_address == null ? wallet.GetChangeAddress() : fee_address,
                            Value = coin_value - bhp_fee
                        }}).ToArray();
                    }
                    addr = feeCoins.Select(p => p.Address).ToList();
                }
            }
            return tx;
        }

        private Coin[] FindUnspentCoins(Wallet wallet, UInt256 asset_id, Fixed8 amount, params UInt160[] from)
        {
            List<Coin> coins = new List<Coin>();
            IEnumerable<UInt160> accounts = from.Length > 0 ? from : wallet.GetAccounts().Where(p => !p.Lock && !p.WatchOnly).Select(p => p.ScriptHash);
            Fixed8 sum = Fixed8.Zero;
            foreach (UInt160 account in accounts)
            {
                Fixed8 accountAmount;
                List<Coin> accountCoins = RpcMethods.GetUnspents(account.ToAddress(), asset_id.ToString(), out accountAmount);
                if (accountCoins != null)
                {
                    coins.AddRange(accountCoins);
                    sum += accountAmount;
                }
            }

            Coin[] unspents_asset = coins.ToArray();
            if (sum < amount) return null;
            if (sum == amount) return unspents_asset;
            Coin[] unspents_ordered = unspents_asset.OrderByDescending(p => p.Output.Value).ToArray();
            int i = 0;
            while (unspents_ordered[i].Output.Value <= amount)
                amount -= unspents_ordered[i++].Output.Value;
            if (amount == Fixed8.Zero)
                return unspents_ordered.Take(i).ToArray();
            else
                return unspents_ordered.Take(i).Concat(new[] { unspents_ordered.Last(p => p.Output.Value >= amount) }).ToArray();

        }
    }
}
