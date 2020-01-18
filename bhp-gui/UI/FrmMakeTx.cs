using System;
using Akka.Actor;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using Bhp.Wallets.BRC6;
using Bhp.Wallets;
using Bhp.Network.P2P.Payloads;
using Bhp.BhpExtensions.Transactions;
using Bhp.SmartContract;
using Bhp.Network.P2P;
using Bhp.Ledger;

namespace Bhp.UI
{
    public partial class FrmMakeTx : Form
    {
        public FrmMakeTx()
        {
            InitializeComponent();            
            timer = new System.Timers.Timer();
            timer.Interval = 15000;
            timer.Elapsed += Timer_Elapsed;            

            path = AppDomain.CurrentDomain.BaseDirectory;
            path = Path.Combine(path, "wallet");

            OpenWallet();
        }

        string path = string.Empty;
        string password = "1";
        UIntBase assetId = UIntBase.Parse("0x13f76fabfe19f3ec7fd54d63179a156bafc44afc53a7f07a7a15f6724c0aa854");
        System.Timers.Timer timer;

        int minValue;
        int maxValue;
        int minTxPreBlock;
        int maxTxPreBlock;
        string[] addressArr;

        private WalletIndexer GetIndexer()
        {
            if (Program.indexer is null)
                Program.indexer = new WalletIndexer(Properties.Settings.Default.Paths.Index);
            return Program.indexer;
        }        

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {            
            GetTransaction();            
        }

        private void btn_create_Click(object sender, EventArgs e)
        {
            ButtonState(false);

            CreateWallet();           
            OpenWallet();

            ButtonState(true);
        }

        private void CreateWallet()
        {
            listBox1.Items.Clear();

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            int count = int.Parse(txt_address_count.Text);
            string walletName = $"{DateTime.Now.ToString("yyyyMMdd")}.json";
            string spath = Path.Combine(path, walletName);
            if (File.Exists(spath))
                return;
            BRC6Wallet wallet = new BRC6Wallet(GetIndexer(), spath);
            wallet.Unlock(password);
            string[] addressList = new string[count];
            for (int i = 0; i < count; i++)
            {
                WalletAccount account = wallet.CreateAccount();
                addressList[i] = account.Address;
                listBox1.Items.Add(account.Address);
                Application.DoEvents();
            }
            wallet.Save();
            File.WriteAllLines(spath + ".snop", addressList);            
        }

        /// <summary>
        /// start timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_start_Click(object sender, EventArgs e)
        {            
            listBox1.Items.Clear();
            if (addressArr == null || addressArr.Length == 0)
            {
                if (!OpenWallet()) return;
            }

            if(Program.CurrentWallet.WalletHeight - 1 != Blockchain.Singleton.HeaderHeight || Blockchain.Singleton.Height != Blockchain.Singleton.HeaderHeight)
            {
                MessageBox.Show("数据尚未同步完成，请稍后...");                
                return;
            }

            minValue = int.Parse(txt_money_min.Text);
            maxValue = int.Parse(txt_money_max.Text);            
            if(!(minValue >= 0 && minValue < maxValue ))
            {
                MessageBox.Show("金额范围错误");                
                return;
            }

            minTxPreBlock = int.Parse(txt_tx_min.Text);
            maxTxPreBlock = int.Parse(txt_tx_max.Text);
            if (!(minTxPreBlock >= 0 && minTxPreBlock < maxTxPreBlock))
            {
                MessageBox.Show("交易个数范围错误");
                return;
            }

            ButtonState(false);

            timer.Start();
            GetTransaction();
        }    

        private void btn_open_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            OpenWallet();
        }

        private bool OpenWallet()
        {
            string[] files = Directory.GetFiles(path, "*.json");            
            if(files.Length == 0)
            {
                MessageBox.Show("请创建钱包");
                return false;
            }

            Wallet wallet;            
            string spath = Path.Combine(path, files[files.Length - 1]);
            if (!File.Exists(spath)) return false;

            BRC6Wallet brc6wallet = new BRC6Wallet(GetIndexer(), spath);
            try
            {
                brc6wallet.Unlock(password);
            }
            catch (Exception ex)
            {
                return false;
            }
            wallet = brc6wallet;

            ChangeWallet(wallet);       
            Bhp.Properties.Settings.Default.LastWalletPath = spath;
            Bhp.Properties.Settings.Default.Save();

            addressArr = File.ReadAllLines(spath + ".snop");
            MessageBox.Show($"钱包 {files[0]} 打开成功");
            return true;
        }

        private void ChangeWallet(Wallet wallet)
        {
            if (Program.CurrentWallet != null)
            {
                if (Program.CurrentWallet is IDisposable disposable)
                    disposable.Dispose();
            }
            Program.CurrentWallet = wallet;            
        }

        /// <summary>
        /// get the random vout address
        /// </summary>
        /// <returns></returns>
        private string GetOutputAddress()
        {
            if (addressArr.Length == 0) return string.Empty;
            int index = 0;
            do
            {
                Random random = new Random();
                index = random.Next(0, addressArr.Length);                

            } while (index >= addressArr.Length);
            System.Threading.Thread.Sleep(15);
            return addressArr[index];
        }

        /// <summary>
        /// get the random vout amount
        /// </summary>
        /// <returns></returns>
        private Fixed8 GetOutputValue()
        {
            decimal value;
            Random random = new Random();
            value = (decimal)(random.Next(minValue * 100, maxValue * 100) * 0.01);
            return Fixed8.FromDecimal(value);
        }

        TransactionContract transactionContract = new TransactionContract();
        /// <summary>
        /// make tx
        /// </summary>
        public void GetTransaction()
        {
            Random random = new Random();
            int txCount = random.Next(minTxPreBlock, maxTxPreBlock);

            for (int i = 0; i < txCount; i++)
            {
                Transaction tx = new ContractTransaction();
                List<TransactionAttribute> attributes = new List<TransactionAttribute>();

                tx.Attributes = attributes.ToArray();
                UInt160 address = GetOutputAddress().ToScriptHash();
                tx.Outputs = new TransactionOutput[] {
                    new TransactionOutput
                    {
                    AssetId = (UInt256)assetId,
                    Value = GetOutputValue(),
                    ScriptHash = address
                    }
                };
                tx.Witnesses = new Witness[0];
                UInt160 changeAddress = GetOutputAddress().ToScriptHash();
                if (tx is ContractTransaction ctx)
                    tx = transactionContract.MakeTransaction(Program.CurrentWallet, ctx, change_address: changeAddress);

                if (tx == null) return;

                ContractParametersContext context = new ContractParametersContext(tx);
                Program.CurrentWallet.Sign(context);
                if (context.Completed)
                {
                    tx.Witnesses = context.GetWitnesses();
                    Program.CurrentWallet.ApplyTransaction(tx);
                    Program.System.LocalNode.Tell(new LocalNode.Relay { Inventory = tx });

                    this.Invoke(new Action(() =>
                    {
                        if (listBox1.Items.Count >= addressArr.Length) listBox1.Items.Clear();
                        listBox1.Items.Add(tx.Hash.ToString());
                    }));
                }
            }
        }

        /// <summary>
        /// stop timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            ButtonState(true);
        }

        /// <summary>
        /// button state control
        /// </summary>
        /// <param name="state"></param>
        private void ButtonState(bool state)
        {
            btn_create.Enabled = state;
            btn_start.Enabled = state;
            btn_open.Enabled = state;
        }

        private void FrmCreateWallets_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();            
        }
    }//end of class
}
