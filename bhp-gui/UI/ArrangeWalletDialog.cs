using Akka.Actor;
using Bhp.BhpExtensions.Fees;
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

        private const int MaxInputCount = 100;
        bool isArrange = false;

        private void button1_Click(object sender, EventArgs e)
        {            
            button1.Enabled = false;
            listBox1.Items.Clear();
            progressBar1.Value = 0;

            timer1.Start();
            timer1_Tick(timer1, new EventArgs());
        }
        
        private void button2_Click(object sender, EventArgs e)
        {                     
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isArrange) return;
            SendTransaction();
        }

        private void SendTransaction()
        {
            isArrange = true;
            this.Invoke(new Action(() => {
                progressBar1.Value = 0;
            }));            

            IEnumerable<WalletAccount> wallets = Program.CurrentWallet.GetAccounts();
            foreach (WalletAccount account in wallets)
            {
                IEnumerable<Coin> allCoins = Program.CurrentWallet.FindUnspentCoins(account.ScriptHash);
                Coin[] coins = TransactionContract.FindUnspentCoins(allCoins, account.ScriptHash);
                if (coins.Length <= 5)
                {
                    listBox1.Items.Add("nothing to do...");
                    timer1.Stop();
                    isArrange = false;
                    button1.Enabled = true;                    
                    return;
                }
                int loopNum = coins.Length / MaxInputCount + ((coins.Length % MaxInputCount == 0) ? 0 : 1);
                double step = 100 / loopNum;

                for (int i = 0; i < loopNum; i++)
                {
                    Transaction tx = new ContractTransaction();
                    tx.Attributes = new TransactionAttribute[0];
                    tx.Witnesses = new Witness[0];

                    List<CoinReference> inputs = new List<CoinReference>();
                    List<TransactionOutput> outputs = new List<TransactionOutput>();

                    Fixed8 sum = Fixed8.Zero;
                    for (int j = 0; j < MaxInputCount; j++)
                    {
                        if (i * MaxInputCount + j >= coins.Length) break;
                        sum += coins[i * MaxInputCount + j].Output.Value;
                        inputs.Add(new CoinReference
                        {
                            PrevHash = coins[i * MaxInputCount + j].Reference.PrevHash,
                            PrevIndex = coins[i * MaxInputCount + j].Reference.PrevIndex
                        });
                    }

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
                    tx.Outputs[0].Value -= BhpTxFee.EstimateTxFee(tx, Blockchain.GoverningToken.Hash);
                   
                    listBox1.Items.Add(SignAndShowMulInformation(tx));
                    listBox1.Refresh();

                    if (i + 1 == loopNum)
                    {
                        this.Invoke(new Action(() => {
                            progressBar1.Value = 100;
                        })); 
                    }
                    else
                    {
                        this.Invoke(new Action(() => {
                            progressBar1.Value = (int)((i + 1) * step);
                        }));                        
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
            isArrange = false;
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
    }
}
