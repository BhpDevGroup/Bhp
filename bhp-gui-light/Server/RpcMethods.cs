using Bhp.Cryptography.ECC;
using Bhp.IO.Json;
using Bhp.Ledger;
using Bhp.Network.P2P.Payloads;
using Bhp.Wallets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bhp.Server
{
    public class RpcMethods
    {
        static HttpService httpService = new HttpService();

        private static JObject CreatePost()
        {
            JObject response = new JObject();
            response["jsonrpc"] = "2.0";
            response["id"] = 1;
            return response;
        }

        public static int GetBlockCount()
        {
            JObject jsonPost = CreatePost();
            jsonPost["method"] = "getblockcount";
            jsonPost["params"] = new JArray() { };
            string result = httpService.HttpPost(jsonPost.ToString());
            if (result == null) return -1;

            try
            {
                JObject result2 = JObject.Parse(result)["result"];
                if (result2 == null)
                {
                    return -1;
                }
                return int.Parse(result2.ToString());
            }
            catch
            {
                return -1;
            }
        }

        public static AccountState GetAccountState(string address)
        {
            JObject jsonPost = CreatePost();
            jsonPost["method"] = "getaccountstate";
            jsonPost["params"] = new JArray() { address };
            string result = httpService.HttpPost(jsonPost.ToString());
            if (result == null) return null;

            try
            {
                JObject result2 = JObject.Parse(result)["result"];
                if (result2 == null)
                {
                    return null;
                }
                JArray balances = (JArray)result2["balances"];
                AccountState accountState = new AccountState(address.ToScriptHash());
                foreach (JObject balance in balances)
                {
                    accountState.Balances.Add(UInt256.Parse(balance["asset"].AsString()), Fixed8.Parse(balance["value"].AsString()));
                }
                return accountState;
            }
            catch
            {
                return null;
            }
        }

        public static AssetState GetAssetState(string assetId)
        {
            JObject jsonPost = CreatePost();
            jsonPost["method"] = "getassetstate";
            jsonPost["params"] = new JArray() { assetId };
            string result = httpService.HttpPost(jsonPost.ToString());
            if (result == null) return null;

            try
            {
                JObject result2 = JObject.Parse(result)["result"];
                if (result2 == null)
                {
                    return null;
                }
                AssetState assetState = new AssetState()
                {
                    AssetId = UInt256.Parse(assetId.ToString()),
                    AssetType = (AssetType)Enum.Parse(typeof(AssetType), result2["type"].AsString()),
                    Name = result2["name"].ToString(),
                    Amount = Fixed8.Parse(result2["amount"].AsString()),
                    Available = Fixed8.Parse(result2["available"].AsString()),
                    Precision = byte.Parse(result2["precision"].AsString()),
                    Owner = ECPoint.Parse(result2["owner"].AsString(), ECCurve.Secp256),
                    Admin = result2["admin"].AsString().ToScriptHash(),
                    Issuer = result2["issuer"].AsString().ToScriptHash(),
                    Expiration = uint.Parse(result2["expiration"].AsString()),
                    IsFrozen = bool.Parse(result2["frozen"].AsString()),
                };
                return assetState;
            }
            catch
            {
                return null;
            }
        }

        public static List<Coin> GetUnspents(string address, string assetId, out Fixed8 amount)
        {
            amount = Fixed8.Zero;

            JObject jsonPost = CreatePost();
            jsonPost["method"] = "getunspents";
            jsonPost["params"] = new JArray() { address, assetId };
            string result = httpService.HttpPost(jsonPost.ToString());
            if (result == null) return null;

            try
            {
                Newtonsoft.Json.Linq.JObject jsons = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(result);

                Newtonsoft.Json.Linq.JObject result2 = (Newtonsoft.Json.Linq.JObject)jsons["result"];
                if (result2 == null)
                {
                    return null;
                }
                Newtonsoft.Json.Linq.JArray balances = (Newtonsoft.Json.Linq.JArray)result2["balance"];

                UInt160 script_hash = address.ToScriptHash();
                UInt256 asset_id = UInt256.Parse(assetId.ToString());
                List<Coin> coins = new List<Coin>();
                foreach (Newtonsoft.Json.Linq.JObject balance in balances)
                {
                    Newtonsoft.Json.Linq.JArray unspents = (Newtonsoft.Json.Linq.JArray)balance["unspent"];
                    amount = Fixed8.Parse(balance["amount"].ToString());
                    foreach (Newtonsoft.Json.Linq.JObject unspent in unspents)
                    {
                        Coin coin = new Coin();
                        coin.Reference = new CoinReference();
                        coin.Output = new TransactionOutput();
                        coin.Reference.PrevHash = UInt256.Parse(unspent["txid"].ToString());
                        coin.Reference.PrevIndex = ushort.Parse(unspent["n"].ToString());
                        coin.Output.AssetId = asset_id;
                        coin.Output.Value = Fixed8.Parse(unspent["value"].ToString());
                        coin.Output.ScriptHash = script_hash;
                        coin.State = CoinState.Confirmed;
                        coins.Add(coin);
                    }
                }
                return coins;
            }
            catch
            {
                return null;
            }
        }

        public static string SendRawTransaction(string txData)
        {
            JObject jsonPost = CreatePost();
            jsonPost["method"] = "sendrawtransaction";
            jsonPost["params"] = new JArray() { txData };
            string result = httpService.HttpPost(jsonPost.ToString());
            if (result == null) return "post error";

            try
            {
                JObject result2 = JObject.Parse(result)["result"];
                if (result2 == null)
                {
                    result2 = JObject.Parse(result)["error"];
                    return $"{result2["code"]},{result2["message"]}";
                }
                return result2.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
