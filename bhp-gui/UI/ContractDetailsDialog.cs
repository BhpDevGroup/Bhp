using Bhp.SmartContract;
using Bhp.Wallets;
using System.Windows.Forms;

namespace Bhp.UI
{
    internal partial class ContractDetailsDialog : Form
    {
        public ContractDetailsDialog(Contract contract)
        {
            InitializeComponent();
            txt_address.Text = contract.ScriptHash.ToAddress();
            txt_scriptHash.Text = contract.ScriptHash.ToString();
            txt_redeemScript.Text = contract.Script.ToHexString();
        }
    }
}
