using Bhp.Ledger;
using Bhp.Network.P2P.Payloads;
using Bhp.VM;
using Bhp.Wallets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bhp.UI
{
    public partial class AssetRenewDialog : Form
    {
        public AssetRenewDialog()
        {
            InitializeComponent();
        }

        public InvocationTransaction GetTransaction()
        {
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                AssetDescriptor asset = (AssetDescriptor)combo_asset.SelectedItem;
                BigInteger year = BigInteger.Parse(txt_year.Text.Trim());
                sb.EmitPush(year);
                sb.EmitPush(asset.AssetId);
                sb.EmitAppCall(UInt160.Parse("0x3188ca534a7bf0c1d94df3e56de1accaf77f7636"));
                return new InvocationTransaction
                {
                    Script = sb.ToArray()
                };
            }
        }

        private void AssetRenewDialog_Load(object sender, EventArgs e)
        {
            foreach (UInt256 asset_id in Program.CurrentWallet.FindUnspentCoins().Select(p => p.Output.AssetId).Distinct())
            {
                if (asset_id.Equals(Blockchain.GoverningToken.Hash) || asset_id.Equals(Blockchain.UtilityToken.Hash)) continue;
                combo_asset.Items.Add(new AssetDescriptor(asset_id));
            }
        }

        private void CheckForm(object sender, EventArgs e)
        {
            bool enabled = combo_asset.SelectedIndex >= 0 && txt_year.TextLength > 0;
            btn_ok.Enabled = enabled;
        }
    }
}
