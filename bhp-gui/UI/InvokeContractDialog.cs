using Bhp.IO.Json;
using Bhp.Ledger;
using Bhp.Network.P2P.Payloads;
using Bhp.Properties;
using Bhp.SmartContract;
using Bhp.UI.Wrappers;
using Bhp.VM;
using Bhp.Wallets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Bhp.Network.RPC.RpcServer;

namespace Bhp.UI
{
    internal partial class InvokeContractDialog : Form
    {
        private InvocationTransaction tx;
        private JObject abi;
        private UInt160 script_hash;
        private ContractParameter[] parameters;
        private ContractParameter[] parameters_abi;

        private static readonly Fixed8 net_fee = Fixed8.FromDecimal(0.001m);
        private List<TransactionAttributeWrapper> temp_signatures = new List<TransactionAttributeWrapper>();
        /// <summary>
        /// invoke 随机数
        /// </summary>
        private static readonly Random rand = new Random();

        public InvokeContractDialog(InvocationTransaction tx = null)
        {
            InitializeComponent();
            this.tx = tx;
            if (tx != null)
            {
                tabControl1.SelectedTab = tabPage_custom;
                textBox6.Text = tx.Script.ToHexString();
            }
            combo_sign.Items.AddRange(Program.CurrentWallet.GetAccounts().Select(p => p.Address).ToArray());
        }

        public InvocationTransaction GetInvokeTransaction()
        {
            List<TransactionAttribute> attributes = new List<TransactionAttribute>();
            byte[] timeStamp = System.Text.ASCIIEncoding.ASCII.GetBytes(DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"));
            byte[] nonce = new byte[8];
            rand.NextBytes(nonce);
            attributes.Add(
                new TransactionAttribute()
                {
                    Usage = TransactionAttributeUsage.Remark,
                    Data = timeStamp.Concat(nonce).ToArray()
                });
            tx.Attributes = tx.Attributes.Concat(attributes).ToArray();

            return Program.CurrentWallet.MakeTransaction(new InvocationTransaction
            {
                Version = tx.Version,
                Script = tx.Script,
                Gas = tx.Gas,
                Attributes = tx.Attributes,
                Inputs = tx.Inputs,
                Outputs = tx.Outputs
            });
        }

        public InvocationTransaction GetTransaction()
        {
            Fixed8 fee = tx.Gas.Equals(Fixed8.Zero) ? net_fee : Fixed8.Zero;
            return Program.CurrentWallet.MakeTransaction(new InvocationTransaction
            {
                Version = tx.Version,
                Script = tx.Script,
                Gas = tx.Gas,
                Attributes = tx.Attributes,
                Inputs = tx.Inputs,
                Outputs = tx.Outputs
            }, fee: fee);
        }

        public InvocationTransaction GetTransaction(UInt160 change_address, Fixed8 fee)
        {
            return Program.CurrentWallet.MakeTransaction(new InvocationTransaction
            {
                Version = tx.Version,
                Script = tx.Script,
                Gas = tx.Gas,
                Attributes = tx.Attributes,
                Inputs = tx.Inputs,
                Outputs = tx.Outputs
            }, change_address: change_address, fee: fee);
        }

        private void UpdateParameters()
        {
            parameters = new[]
            {
                new ContractParameter
                {
                    Type = ContractParameterType.String,
                    Value = comboBox1.SelectedItem
                },
                new ContractParameter
                {
                    Type = ContractParameterType.Array,
                    Value = parameters_abi
                }
            };
        }

        private void UpdateScript()
        {
            if (parameters.Any(p => p.Value == null)) return;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitAppCall(script_hash, parameters);
                textBox6.Text = sb.ToArray().ToHexString();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = UInt160.TryParse(textBox1.Text, out _);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            script_hash = UInt160.Parse(textBox1.Text);
            ContractState contract = Blockchain.Singleton.Store.GetContracts().TryGet(script_hash);
            if (contract == null) return;
            parameters = contract.ParameterList.Select(p => new ContractParameter(p)).ToArray();
            textBox2.Text = contract.Name;
            textBox3.Text = contract.CodeVersion;
            textBox4.Text = contract.Author;
            textBox5.Text = string.Join(", ", contract.ParameterList);
            button2.Enabled = parameters.Length > 0;
            UpdateScript();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (ParametersEditor dialog = new ParametersEditor(parameters))
            {
                dialog.ShowDialog();
            }
            UpdateScript();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button5.Enabled = textBox6.TextLength > 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte[] script;
            try
            {
                script = textBox6.Text.Trim().HexToBytes();
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            if (tx == null) tx = new InvocationTransaction();
            tx.Version = 1;
            tx.Script = script;
            tx.Attributes = temp_signatures.Select(p => p.Unwrap()).ToArray();
            if (tx.Inputs == null) tx.Inputs = new CoinReference[0];
            if (tx.Outputs == null) tx.Outputs = new TransactionOutput[0];
            if (tx.Witnesses == null) tx.Witnesses = new Witness[0];
            if (tx.Attributes != null)
            {
                try
                {
                    ContractParametersContext context;
                    context = new ContractParametersContext(tx);
                    Program.CurrentWallet.Sign(context);
                    tx.Witnesses = context.GetWitnesses();
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show(Strings.UnsynchronizedBlock);
                    return;
                }
            }

            using (ApplicationEngine engine = ApplicationEngine.Run(tx.Script, tx, testMode: true))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"VM State: {engine.State}");
                sb.AppendLine($"Gas Consumed: {engine.GasConsumed}");
                sb.AppendLine($"Evaluation Stack: {new JArray(engine.ResultStack.Select(p => p.ToParameter().ToJson()))}");
                JObject notifications = engine.Service.Notifications.Select(q =>
                {
                    JObject notification = new JObject();
                    notification["contract"] = q.ScriptHash.ToString();
                    try
                    {
                        notification["state"] = q.State.ToParameter().ToJson();
                    }
                    catch (InvalidOperationException)
                    {
                        notification["state"] = "error: recursive reference";
                    }
                    return notification;
                }).ToArray();
                sb.AppendLine($"Notifications: {notifications}");
                textBox7.Text = sb.ToString();
                if (!engine.State.HasFlag(VMState.FAULT))
                {
                    tx.Gas = engine.GasConsumed - Fixed8.FromDecimal(10);
                    if (tx.Gas < Fixed8.Zero) tx.Gas = Fixed8.Zero;
                    tx.Gas = tx.Gas.Ceiling();
                    label7.Text = tx.Gas + " gas";
                    button3.Enabled = true;
                }
                else
                {
                    MessageBox.Show(Strings.ExecutionFailed);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            textBox6.Text = File.ReadAllBytes(openFileDialog1.FileName).ToHexString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() != DialogResult.OK) return;
            abi = JObject.Parse(File.ReadAllText(openFileDialog2.FileName));
            script_hash = UInt160.Parse(abi["hash"].AsString());
            textBox8.Text = script_hash.ToString();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(((JArray)abi["functions"]).Select(p => p["name"].AsString()).Where(p => p != abi["entrypoint"].AsString()).ToArray());
            textBox9.Clear();
            button8.Enabled = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (ParametersEditor dialog = new ParametersEditor(parameters_abi))
            {
                dialog.ShowDialog();
            }
            UpdateParameters();
            UpdateScript();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(comboBox1.SelectedItem is string method)) return;
            JArray functions = (JArray)abi["functions"];
            JObject function = functions.First(p => p["name"].AsString() == method);
            JArray _params = (JArray)function["parameters"];
            parameters_abi = _params.Select(p => new ContractParameter(p["type"].AsEnum<ContractParameterType>())).ToArray();
            textBox9.Text = string.Join(", ", _params.Select(p => p["name"].AsString()));
            button8.Enabled = parameters_abi.Length > 0;
            UpdateParameters();
            UpdateScript();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (combo_sign.SelectedItem.ToString() == "")
            {
                MessageBox.Show("Please choose the address");
                return;
            }
            var index = combo_sign.SelectedIndex;
            temp_signatures.Add(new TransactionAttributeWrapper
            {
                Usage = TransactionAttributeUsage.Script,
                Data = combo_sign.SelectedItem.ToString().ToScriptHash().ToArray()
            });
            MessageBox.Show("Success!");
            combo_sign.Items.RemoveAt(index);
            if (combo_sign.Items.Count > 0)
            {
                combo_sign.SelectedIndex = 0;
            }
            else
            {
                combo_sign.SelectedText = "";
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            temp_signatures.Clear();
            combo_sign.Items.Clear();
            combo_sign.SelectedText = "";
            combo_sign.Items.AddRange(Program.CurrentWallet.GetAccounts().Select(p => p.Address).ToArray());
            MessageBox.Show("Success!");
        }
    }
}
