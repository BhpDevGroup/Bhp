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
using System.Linq;
using System.Windows.Forms;

namespace Bhp.UI
{
    public partial class ArrangeWalletDialog : Form
    {
        public ArrangeWalletDialog()
        {
            InitializeComponent();
            foreach (UInt256 asset_id in Program.CurrentWallet.FindUnspentCoins().Select(p => p.Output.AssetId).Distinct())
            {
                combo_asset.Items.Add(new AssetDescriptor(asset_id));
            }
            foreach (string s in Properties.Settings.Default.BRC20Watched)
            {
                UInt160 asset_id = UInt160.Parse(s);
                try
                {
                    combo_asset.Items.Add(new AssetDescriptor(asset_id));
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }
        }

        private const int MaxInputCount = 100;

        private void btn_arrange_Click(object sender, EventArgs e)
        {
            combo_asset.Enabled = false;
            btn_arrange.Enabled = false;
            listBox1.Items.Clear();
            progressBar1.Value = 0;

            timer1.Start();
            timer1_Tick(timer1, new EventArgs());
        }
        
        private void btn_close_Click(object sender, EventArgs e)
        {
            StopArrange();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy) return;
            Coin[] coins = FindCoins();            
            backgroundWorker1.RunWorkerAsync(coins);
        }

        UInt160 currentAddress;
        private Coin[] FindCoins()
        {
            IEnumerable<WalletAccount> wallets = Program.CurrentWallet.GetAccounts();
            List<Coin> allCoin = new List<Coin>();
            foreach (WalletAccount account in wallets)
            {
                IEnumerable<Coin> allCoins = Program.CurrentWallet.FindUnspentCoins(account.ScriptHash);
                Coin[] coins = TransactionContract.FindUnspentCoins(allCoins, (UInt256)asset.AssetId);
                allCoin.AddRange(coins);
                if (allCoin.Count <= 5) continue;
                currentAddress = account.ScriptHash;
                return allCoin.ToArray();
            }
            StopArrange();
            return new Coin[0];
        }

        private void StopArrange()
        {
            timer1.Stop();
            backgroundWorker1.CancelAsync();
            combo_asset.Enabled = true;
            btn_arrange.Enabled = true;
        }

        private void SendTransaction(Coin[] coins)
        {            
            if (coins.Length <= 5) return;
            backgroundWorker1.ReportProgress(progressBar1.Minimum);

            int loopNum = coins.Length / MaxInputCount + ((coins.Length % MaxInputCount == 0) ? 0 : 1);
            double step = (double)100 / loopNum;

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
                    AssetId = (UInt256)asset.AssetId,
                    ScriptHash = currentAddress,
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
                if ((UInt256)asset.AssetId == Blockchain.GoverningToken.Hash)
                {
                    tx.Outputs[0].Value -= BhpTxFee.EstimateTxFee(tx, Blockchain.GoverningToken.Hash);
                }

                string msg = SignAndShowMulInformation(tx);

                if (i + 1 == loopNum)
                {
                    backgroundWorker1.ReportProgress(progressBar1.Maximum, msg);
                }
                else
                {
                    backgroundWorker1.ReportProgress((int)((i + 1) * step), msg);
                }
                System.Threading.Thread.Sleep(1000);
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
                tx.Witnesses = context.GetWitnesses();

                if (tx.Size > Transaction.MaxTransactionSize)
                {
                    return Strings.TxTooLarge;
                }

                Program.CurrentWallet.ApplyTransaction(tx);
                Program.System.LocalNode.Tell(new LocalNode.Relay { Inventory = tx });
                return tx.Hash.ToString();
            }
            else
            {
                return context.ToString();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            SendTransaction((Coin[])e.Argument);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null) listBox1.Items.Add(e.UserState.ToString());
            if (e.ProgressPercentage <= progressBar1.Maximum)
            {
                progressBar1.Value = e.ProgressPercentage;
            }
        }

        AssetDescriptor asset = null;
        private void combo_asset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combo_asset.SelectedIndex < 0)
            {
                asset = null;
                btn_arrange.Enabled = false;                
            }
            else
            {
                asset = (AssetDescriptor)combo_asset.SelectedItem;
                btn_arrange.Enabled = true;
            }
        }
    }
}
