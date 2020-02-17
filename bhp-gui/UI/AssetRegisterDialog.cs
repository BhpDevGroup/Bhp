using Bhp.Cryptography.ECC;
using Bhp.Network.P2P.Payloads;
using Bhp.SmartContract;
using Bhp.VM;
using Bhp.Wallets;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Bhp.UI
{
    public partial class AssetRegisterDialog : Form
    {
        public AssetRegisterDialog()
        {
            InitializeComponent();
        }

        public InvocationTransaction GetTransaction()
        {
            AssetType asset_type = (AssetType)combo_asset.SelectedItem;
            string name = string.IsNullOrWhiteSpace(txt_name.Text) ? string.Empty : $"[{{\"lang\":\"{CultureInfo.CurrentCulture.Name}\",\"name\":\"{txt_name.Text}\"}}]";
            Fixed8 amount = checkBox1.Checked ? Fixed8.Parse(txt_capped.Text) : -Fixed8.Satoshi;
            byte precision = (byte)numericUpDown1.Value;
            ECPoint owner = (ECPoint)combo_owner.SelectedItem;
            UInt160 admin = combo_admin.Text.ToScriptHash();
            UInt160 issuer = combo_issuer.Text.ToScriptHash();
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitSysCall("Bhp.Asset.Create", asset_type, name, amount, precision, owner, admin, issuer);
                return new InvocationTransaction
                {
                    Attributes = new[]
                    {
                        new TransactionAttribute
                        {
                            Usage = TransactionAttributeUsage.Script,
                            Data = Contract.CreateSignatureRedeemScript(owner).ToScriptHash().ToArray()
                        }
                    },
                    Script = sb.ToArray()
                };
            }
        }

        private void AssetRegisterDialog_Load(object sender, EventArgs e)
        {
            combo_asset.Items.AddRange(new object[] { AssetType.Share, AssetType.Token });
            combo_owner.Items.AddRange(Program.CurrentWallet.GetAccounts().Where(p => !p.WatchOnly && p.Contract.Script.IsSignatureContract()).Select(p => p.GetKey().PublicKey).ToArray());
            combo_admin.Items.AddRange(Program.CurrentWallet.GetAccounts().Where(p => !p.WatchOnly).Select(p => p.Address).ToArray());
            combo_issuer.Items.AddRange(Program.CurrentWallet.GetAccounts().Where(p => !p.WatchOnly).Select(p => p.Address).ToArray());
        }

        private void combo_asset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(combo_asset.SelectedItem is AssetType assetType)) return;
            numericUpDown1.Enabled = assetType != AssetType.Share;
            if (!numericUpDown1.Enabled) numericUpDown1.Value = 0;
            CheckForm(sender, e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txt_capped.Enabled = checkBox1.Checked;
            CheckForm(sender, e);
        }

        private void CheckForm(object sender, EventArgs e)
        {
            bool enabled = combo_asset.SelectedIndex >= 0 &&
                              txt_name.TextLength > 0 &&
                              (!checkBox1.Checked || txt_capped.TextLength > 0) &&
                              combo_owner.SelectedIndex >= 0 &&
                              !string.IsNullOrWhiteSpace(combo_admin.Text) &&
                              !string.IsNullOrWhiteSpace(combo_issuer.Text);
            if (enabled)
            {
                try
                {
                    combo_admin.Text.ToScriptHash();
                    combo_issuer.Text.ToScriptHash();
                }
                catch (FormatException)
                {
                    enabled = false;
                }
            }
            button1.Enabled = enabled;
        }
    }
}
