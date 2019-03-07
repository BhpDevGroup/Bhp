using Akka.Actor;
using Bhp.BhpExtensions.Transactions;
using Bhp.Ledger;
using Bhp.Network.P2P;
using Bhp.Network.P2P.Payloads;
using Bhp.Properties;
using Bhp.SmartContract;
using Bhp.Wallets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bhp.UI
{
    public partial class ArrangeWalletDialog : Form
    {
        public ArrangeWalletDialog()
        {
            InitializeComponent();
        }

        private const int MaxInputCount = 1000;

        private void SendTransaction()
        {
            IEnumerable<WalletAccount> wallets = Program.CurrentWallet.GetAccounts();
            List<string> txMsg = new List<string>();

            foreach (WalletAccount account in wallets)
            {
                IEnumerable<Coin> allCoins = Program.CurrentWallet.FindUnspentCoins(account.ScriptHash);
                Coin[] coins = TransactionContract.FindUnspentCoins(allCoins, account.ScriptHash);

                int loopNum = coins.Length / MaxInputCount + (coins.Length % MaxInputCount) == 0 ? 0 : (0 + 1);

                //if (1000 * loopNum < coins.Length) loopNum++;

                for (int i = 0; i < loopNum; i++)
                {
                    Transaction tx = new ContractTransaction();
                    tx.Attributes = new TransactionAttribute[0];
                    tx.Witnesses = new Witness[0];

                    List<CoinReference> inputs = new List<CoinReference>();
                    List<TransactionOutput> outputs = new List<TransactionOutput>();

                    Fixed8 sum = Fixed8.Zero;
                    for (int j = 0; j < 1000; j++)
                    {
                        if (i * 1000 + j >= coins.Length) break;
                        sum += coins[i * 1000 + j].Output.Value;
                        inputs.Add(new CoinReference
                        {
                            PrevHash = coins[i * 1000 + j].Reference.PrevHash,
                            PrevIndex = coins[i * 1000 + j].Reference.PrevIndex
                        });
                    }

                    if (sum == Fixed8.Zero) break;

                    tx.Inputs = inputs.ToArray();

                    outputs.Add(new TransactionOutput
                    {
                        AssetId = Blockchain.GoverningToken.Hash,
                        ScriptHash = account.ScriptHash,
                        Value = sum

                    });
                    if (tx.SystemFee > Fixed8.Zero)
                    {
                        outputs.Add(new TransactionOutput
                        {
                            AssetId = Blockchain.UtilityToken.Hash,
                            Value = tx.SystemFee
                        });
                    }
                    tx.Outputs = outputs.ToArray();

                    tx.Outputs[0].Value -= Bhp.BhpExtensions.Fees.BhpTxFee.EstimateTxFee(tx, Blockchain.GoverningToken.Hash);

                    txMsg.Add(SignAndShowMulInformation(tx));
                }
            }
        }

        private string SignAndShowMulInformation(Transaction tx)
        {
            if (tx == null)
            {
                return Strings.InsufficientFunds;
            }
            ContractParametersContext context;
            try
            {
                context = new ContractParametersContext(tx);
            }
            catch (InvalidOperationException)
            {
                return Strings.UnsynchronizedBlock;
            }
            Program.CurrentWallet.Sign(context);
            if (context.Completed)
            {
                context.Verifiable.Witnesses = context.GetWitnesses();
                Program.CurrentWallet.ApplyTransaction(tx);
                Program.System.LocalNode.Tell(new LocalNode.Relay { Inventory = tx });
                return tx.Hash.ToString();
            }
            else
            {
                return context.ToString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
