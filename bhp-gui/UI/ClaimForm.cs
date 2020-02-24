using Akka.Actor;
using Bhp.IO.Actors;
using Bhp.Ledger;
using Bhp.Network.P2P.Payloads;
using Bhp.Persistence;
using Bhp.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Bhp.UI
{
    public partial class ClaimForm : Form
    {
        private IActorRef actor;

        public ClaimForm()
        {
            InitializeComponent();
            this.Width = 430;
            this.Height = 208;
        }

        private void CalculateBonusUnavailable(uint height)
        {
            var unspent = Program.CurrentWallet.FindUnspentCoins()
                .Where(p => p.Output.AssetId.Equals(Blockchain.GoverningToken.Hash))
                .Select(p => p.Reference);

            ICollection<CoinReference> references = new HashSet<CoinReference>();

            foreach (var group in unspent.GroupBy(p => p.PrevHash))
            {
                if (!Blockchain.Singleton.ContainsTransaction(group.Key))
                    continue; // not enough of the chain available
                foreach (var reference in group)
                    references.Add(reference);
            }

            using (Snapshot snapshot = Blockchain.Singleton.GetSnapshot())
            {
                txt_unavailable.Text = snapshot.CalculateBonus(references, height).ToString();
            }
        }

        private void ClaimForm_Load(object sender, EventArgs e)
        {
            using (Snapshot snapshot = Blockchain.Singleton.GetSnapshot())
            {
                Fixed8 bonus_available = snapshot.CalculateBonus(Program.CurrentWallet.GetUnclaimedCoins().Select(p => p.Reference));
                txt_available.Text = bonus_available.ToString();
                if (bonus_available == Fixed8.Zero) btn_claimAll.Enabled = false;
                CalculateBonusUnavailable(snapshot.Height + 1);
            }
            BindAddresses();
            actor = Program.System.ActorSystem.ActorOf(EventWrapper<Blockchain.PersistCompleted>.Props(Blockchain_PersistCompleted));
        }

        private void ClaimForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.System.ActorSystem.Stop(actor);
        }

        private void Blockchain_PersistCompleted(Blockchain.PersistCompleted e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Blockchain.PersistCompleted>(Blockchain_PersistCompleted), e);
            }
            else
            {
                CalculateBonusUnavailable(e.Block.Index + 1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CoinReference[] claims = Program.CurrentWallet.GetUnclaimedCoins().Select(p => p.Reference).ToArray();
            if (claims.Length == 0) return;
            var address = combo_claimTo.Text;
            CoinReference[] getClaims;

            if (claims.Length > 50)
            {
                getClaims = claims.Take(50).ToArray();
            }
            else
            {
                getClaims = claims;
            }

            try
            {
                using (Snapshot snapshot = Blockchain.Singleton.GetSnapshot())
                    Helper.SignAndShowInformation(new ClaimTransaction
                    {
                        Claims = getClaims,
                        Attributes = new TransactionAttribute[0],
                        Inputs = new CoinReference[0],
                        Outputs = new[]
                        {
                        new TransactionOutput
                        {
                            AssetId = Blockchain.UtilityToken.Hash,
                            Value = snapshot.CalculateBonus(getClaims),
                            ScriptHash = address.ToScriptHash()
                        }
                    }
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            Close();
        }

        private void BindAddresses()
        {
            var accounts = Program.CurrentWallet.GetAccounts();
            var addresses = accounts.Select(c => c.ScriptHash.ToAddress()).ToArray();
            combo_claimTo.Items.Clear();
            combo_claimTo.Items.AddRange(addresses);
            combo_claimTo.SelectedIndex = 0;
        }

        private void combo_address_TextChanged(object sender, EventArgs e)
        {
            try
            {
                combo_claimTo.Text.ToScriptHash();
                btn_claimAll.Enabled = true;
            }
            catch (FormatException)
            {
                btn_claimAll.Enabled = false;
            }
        }
    }
}
