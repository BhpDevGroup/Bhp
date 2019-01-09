using Bhp.Wallets;
using Bhp.Wallets.BRC6;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BhpDemo
{
    public class AddressDemo
    {
        public string CreatePublicKey()
        {
            byte[] privateKey = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(privateKey);
            }

            KeyPair keyPair = new KeyPair(privateKey);
            return keyPair.ToString();
        }

        public string CreateAddress()
        {
            if (File.Exists("wallet1.json"))
            {
                File.Delete("wallet1.json");
            }

            BRC6Wallet wallet = new BRC6Wallet(new WalletIndexer("/wallet_index"),"wallet1.json");
            wallet.Unlock("1");
            WalletAccount account = wallet.CreateAccount();
            wallet.Save();
            return account.Address;
        }
    }
}
