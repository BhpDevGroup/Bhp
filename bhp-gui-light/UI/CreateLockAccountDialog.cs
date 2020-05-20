using Bhp.SmartContract;
using Bhp.VM;
using Bhp.Wallets;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Bhp.UI
{
    internal partial class CreateLockAccountDialog : Form
    {
        public CreateLockAccountDialog()
        {
            InitializeComponent();
            combo_account.Items.AddRange(Program.CurrentWallet.GetAccounts().Where(p => !p.WatchOnly && p.Contract.Script.IsStandardContract()).Select(p => p.GetKey()).ToArray());
        }

        public Contract GetContract()
        {
            uint timestamp = dtp_unlockTime.Value.ToTimestamp();

            if (dtp_unlockTime.Value <= DateTime.Now) return null;//BY BHP

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(GetKey().PublicKey);
                sb.EmitPush(timestamp);
                // Lock 2.0 in mainnet tx:4e84015258880ced0387f34842b1d96f605b9cc78b308e1f0d876933c2c9134b
                sb.EmitAppCall(UInt160.Parse("0xdc5f72c06e6ea9dbacd8ddf7bd80f92683966081"));
                return Contract.Create(new[] { ContractParameterType.Signature }, sb.ToArray());
            }
        }

        public KeyPair GetKey()
        {
            return (KeyPair)combo_account.SelectedItem;
        }

        private void combo_account_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_ok.Enabled = combo_account.SelectedIndex >= 0;
        }
    }
}
