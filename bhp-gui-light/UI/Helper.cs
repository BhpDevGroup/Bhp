using Akka.Actor;
using Bhp.Cryptography.ECC;
using Bhp.Network.P2P;
using Bhp.Network.P2P.Payloads;
using Bhp.Properties;
using Bhp.Server;
using Bhp.SmartContract;
using Bhp.Wallets;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Bhp.IO;

namespace Bhp.UI
{
    internal static class Helper
    {
        private static Dictionary<Type, Form> tool_forms = new Dictionary<Type, Form>();

        private static void Helper_FormClosing(object sender, FormClosingEventArgs e)
        {
            tool_forms.Remove(sender.GetType());
        }

        public static void Show<T>() where T : Form, new()
        {
            Type t = typeof(T);
            if (!tool_forms.ContainsKey(t))
            {
                tool_forms.Add(t, new T());
                tool_forms[t].FormClosing += Helper_FormClosing;
            }
            tool_forms[t].Show();
            tool_forms[t].Activate();
        }

        public static void SignAndShowInformation(Transaction tx, List<string> inputAddress = null)
        {
            if (tx == null)
            {
                MessageBox.Show(Strings.InsufficientFunds);
                return;
            }
            if (inputAddress == null)
            {
                MessageBox.Show(Strings.IncompletedSignatureMessage);
                return;
            }
            WalletContractParametersContext context;
            try
            {
                context = new WalletContractParametersContext(tx);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(Strings.UnsynchronizedBlock);
                return;
            }
            Sign(context, inputAddress);
            if (context.Completed)
            {
                tx.Witnesses = context.GetWitnesses();

                if (tx.Size > Transaction.MaxTransactionSize)
                {
                    MessageBox.Show(Strings.TxTooLarge);
                    return;
                }

                Program.CurrentWallet.ApplyTransaction(tx);
                string result = RpcMethods.SendRawTransaction(tx.ToArray().ToHexString());
                if (result == "true")
                {
                    InformationBox.Show(tx.Hash.ToString(), Strings.SendTxSucceedMessage, Strings.SendTxSucceedTitle);
                }
                else
                {
                    result = $"{result}\r\ntxHex : {tx.ToArray().ToHexString()}\r\ntxId : {tx.Hash}";
                    InformationBox.Show(result, Strings.ErrorMessage, Strings.SendTxFailTitle);
                }
            }
            else
            {
                InformationBox.Show(context.ToString(), Strings.IncompletedSignatureMessage, Strings.IncompletedSignatureTitle);
            }
        }

        public static bool Sign(WalletContractParametersContext context, List<string> inputAddress)
        {
            List<UInt160> address = new List<UInt160>();
            foreach (string addr in inputAddress)
            {
                address.Add(addr.ToScriptHash());
            }
            context.ScriptHashes = address;

            bool fSuccess = false;
            foreach (string addr in inputAddress)
            {
                WalletAccount account = Program.CurrentWallet.GetAccount(addr.ToScriptHash());
                if (account?.HasKey != true) continue;
                KeyPair key = account.GetKey();
                byte[] signature = context.Verifiable.Sign(key);
                fSuccess |= context.AddSignature(account.Contract, key.PublicKey, signature);
            }
            return fSuccess;
        }
    }
}
