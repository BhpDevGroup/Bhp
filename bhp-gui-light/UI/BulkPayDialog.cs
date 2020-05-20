using Bhp.Properties;
using Bhp.Wallets;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Bhp.UI
{
    internal partial class BulkPayDialog : Form
    {
        public BulkPayDialog(AssetDescriptor asset = null)
        {
            InitializeComponent();
            if (asset == null)
            {
                foreach (UInt256 asset_id in Program.CurrentWallet.FindUnspentCoins().Select(p => p.Output.AssetId).Distinct())
                {
                    combo_asset.Items.Add(new AssetDescriptor(asset_id));
                }
                foreach (string s in Settings.Default.BRC20Watched)
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
            else
            {
                combo_asset.Items.Add(asset);
                combo_asset.SelectedIndex = 0;
                combo_asset.Enabled = false;
            }
        }

        public TxOutListBoxItem[] GetOutputs()
        {
            AssetDescriptor asset = (AssetDescriptor)combo_asset.SelectedItem;
            return txt_payto.Lines.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p =>
            {
                string[] line = p.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                return new TxOutListBoxItem
                {
                    AssetName = asset.AssetName,
                    AssetId = asset.AssetId,
                    Value = BigDecimal.Parse(line[1], asset.Decimals),
                    ScriptHash = line[0].ToScriptHash()
                };
            }).Where(p => p.Value.Value != 0).ToArray();
        }

        private void combo_asset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combo_asset.SelectedItem is AssetDescriptor asset)
            {
                txt_balance.Text = Program.CurrentWallet.GetAvailable(asset.AssetId).ToString();
            }
            else
            {
                txt_balance.Text = "";
            }
            txt_payto_TextChanged(this, EventArgs.Empty);
        }

        private void txt_payto_TextChanged(object sender, EventArgs e)
        {
            btn_ok.Enabled = combo_asset.SelectedIndex >= 0 && txt_payto.TextLength > 0;
        }
    }
}
