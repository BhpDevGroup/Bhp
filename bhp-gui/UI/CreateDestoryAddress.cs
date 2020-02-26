using Bhp.SmartContract;
using Bhp.VM;
using Bhp.Wallets;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Bhp.UI
{
    internal partial class CreateDestoryAddress : Form
    {
        public CreateDestoryAddress()
        {
            InitializeComponent();
            combo_account.Items.AddRange(Program.CurrentWallet.GetAccounts().Where(p => !p.WatchOnly && p.Contract.Script.IsStandardContract()).Select(p => p.GetKey()).ToArray());
        }

        public Contract GetContract()
        {
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitAppCall(UInt160.Parse("0x5994faf58da993048021666b858b36b10ecdc718"));
                return Contract.Create(new[] { ContractParameterType.Signature }, sb.ToArray());
            }
        }

        public KeyPair GetKey()
        {
            return (KeyPair)combo_account.SelectedItem;
        }

        private void combo_account_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_create.Enabled = combo_account.SelectedIndex >= 0;
        }
    }
}
