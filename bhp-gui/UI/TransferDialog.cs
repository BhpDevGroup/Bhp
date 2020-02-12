using Bhp.BhpExtensions.Transactions;
using Bhp.Network.P2P.Payloads;
using Bhp.Properties;
using Bhp.SmartContract;
using Bhp.VM;
using Bhp.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using VMArray = Bhp.VM.Types.Array;

namespace Bhp.UI
{
    public partial class TransferDialog : Form
    {
        private string remark = "";

        //By BHP
        TransactionContract transactionContract = new TransactionContract();

        public Fixed8 Fee => Fixed8.Parse(txt_fee.Text);
        public UInt160 ChangeAddress => ((string)combo_change.SelectedItem).ToScriptHash();

        public TransferDialog()
        {
            InitializeComponent();
            this.txOutListBox1.Width = 555;
            this.txOutListBox1.Height = 245;
            txt_fee.Text = "0";
            combo_change.Items.AddRange(Program.CurrentWallet.GetAccounts().Select(p => p.Address).ToArray());
            combo_change.SelectedItem = Program.CurrentWallet.GetChangeAddress().ToAddress();
            combo_from.Items.AddRange(Program.CurrentWallet.GetAccounts().Select(p => p.Address).ToArray());
        }

        public Transaction GetTransaction()
        {
            var cOutputs = txOutListBox1.Items.Where(p => p.AssetId is UInt160).GroupBy(p => new
            {
                AssetId = (UInt160)p.AssetId,
                Account = p.ScriptHash
            }, (k, g) => new
            {
                k.AssetId,
                Value = g.Aggregate(BigInteger.Zero, (x, y) => x + y.Value.Value),
                k.Account
            }).ToArray();
            Transaction tx;
            List<TransactionAttribute> attributes = new List<TransactionAttribute>();

            if (LockAttribute != null)//by bhp lock utxo
            {
                if (MessageBox.Show("确认锁仓？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    attributes.Add(LockAttribute);
                }
            }

            UInt160 fromAddress = null;
            if (combo_from.SelectedItem != null)
            {
                fromAddress = ((string)combo_from.SelectedItem).ToScriptHash();
            }

            if (cOutputs.Length == 0)
            {
                tx = new ContractTransaction();
            }
            else
            {
                UInt160[] addresses;
                if (combo_from.SelectedItem == null)
                {
                    addresses = Program.CurrentWallet.GetAccounts().Select(p => p.ScriptHash).ToArray();
                }
                else
                {
                    addresses = Program.CurrentWallet.GetAccounts().Where(e => e.ScriptHash.Equals(fromAddress)).Select(p => p.ScriptHash).ToArray();
                }
                HashSet<UInt160> sAttributes = new HashSet<UInt160>();
                using (ScriptBuilder sb = new ScriptBuilder())
                {
                    foreach (var output in cOutputs)
                    {
                        byte[] script;
                        using (ScriptBuilder sb2 = new ScriptBuilder())
                        {
                            foreach (UInt160 address in addresses)
                                sb2.EmitAppCall(output.AssetId, "balanceOf", address);
                            sb2.Emit(OpCode.DEPTH, OpCode.PACK);
                            script = sb2.ToArray();
                        }
                        using (ApplicationEngine engine = ApplicationEngine.Run(script))
                        {
                            if (engine.State.HasFlag(VMState.FAULT)) return null;
                            var balances = ((VMArray)engine.ResultStack.Pop()).AsEnumerable().Reverse().Zip(addresses, (i, a) => new
                            {
                                Account = a,
                                Value = i.GetBigInteger()
                            }).ToArray();
                            BigInteger sum = balances.Aggregate(BigInteger.Zero, (x, y) => x + y.Value);
                            if (sum < output.Value) return null;
                            if (sum != output.Value)
                            {
                                balances = balances.OrderByDescending(p => p.Value).ToArray();
                                BigInteger amount = output.Value;
                                int i = 0;
                                while (balances[i].Value <= amount)
                                    amount -= balances[i++].Value;
                                if (amount == BigInteger.Zero)
                                    balances = balances.Take(i).ToArray();
                                else
                                    balances = balances.Take(i).Concat(new[] { balances.Last(p => p.Value >= amount) }).ToArray();
                                sum = balances.Aggregate(BigInteger.Zero, (x, y) => x + y.Value);
                            }
                            sAttributes.UnionWith(balances.Select(p => p.Account));
                            for (int i = 0; i < balances.Length; i++)
                            {
                                BigInteger value = balances[i].Value;
                                if (i == 0)
                                {
                                    BigInteger change = sum - output.Value;
                                    if (change > 0) value -= change;
                                }
                                sb.EmitAppCall(output.AssetId, "transfer", balances[i].Account, output.Account, value);
                                sb.Emit(OpCode.THROWIFNOT);
                            }
                        }
                    }
                    tx = new InvocationTransaction
                    {
                        Version = 1,
                        Script = sb.ToArray()
                    };
                }
                attributes.AddRange(sAttributes.Select(p => new TransactionAttribute
                {
                    Usage = TransactionAttributeUsage.Script,
                    Data = p.ToArray()
                }));
            }
            if (!string.IsNullOrEmpty(remark))
                attributes.Add(new TransactionAttribute
                {
                    Usage = TransactionAttributeUsage.Remark,
                    Data = Encoding.UTF8.GetBytes(remark)
                });
            tx.Attributes = attributes.ToArray();
            tx.Outputs = txOutListBox1.Items.Where(p => p.AssetId is UInt256).Select(p => p.ToTxOutput()).ToArray();
            tx.Witnesses = new Witness[0];
            if (tx is ContractTransaction ctx)
                //tx = Program.CurrentWallet.MakeTransaction(ctx, change_address: ChangeAddress, fee: Fee);
                tx = transactionContract.MakeTransaction(Program.CurrentWallet, ctx, from: fromAddress, change_address: ChangeAddress, fee: Fee);
            return tx;
        }

        private void txOutListBox1_ItemsChanged(object sender, EventArgs e)
        {
            //button3.Enabled = txOutListBox1.ItemCount > 0;
            button3.Enabled = txOutListBox1.ItemCount > 0 && Program.CurrentWallet.WalletHeight - 1 == Ledger.Blockchain.Singleton.HeaderHeight && Ledger.Blockchain.Singleton.Height == Ledger.Blockchain.Singleton.HeaderHeight;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            remark = InputBox.Show(Strings.EnterRemarkMessage, Strings.EnterRemarkTitle, remark);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            groupBox1.Visible = true;
            this.Height = 560;
        }

        TransactionAttribute LockAttribute = null;
        DateTime lockTime = new DateTime();
        private void btn_lock_Click(object sender, EventArgs e)
        {
            using (LockUTXODialog dialog = new LockUTXODialog(lockTime))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.GetUXTOLockTime <= DateTime.Now)
                    {
                        MessageBox.Show(Strings.LockTime);
                        return;
                    }
                    lockTime = dialog.GetUXTOLockTime;
                    TransactionContract transactionContract = new TransactionContract();
                    LockAttribute = transactionContract.MakeLockTransactionScript(lockTime.ToTimestamp());
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button3.Enabled = txOutListBox1.ItemCount > 0 && Program.CurrentWallet.WalletHeight - 1 == Ledger.Blockchain.Singleton.HeaderHeight && Ledger.Blockchain.Singleton.Height == Ledger.Blockchain.Singleton.HeaderHeight;
        }
    }
}
