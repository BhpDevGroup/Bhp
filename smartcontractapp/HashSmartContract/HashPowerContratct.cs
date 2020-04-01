using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System;
using System.Numerics;

namespace HashSmartContract
{
    public class HashPowerContratct : SmartContract
    {

        private static readonly byte[] Owner = "ATe3wDE9MPQXZuvhgPREdQNYkiCBF7JShY".ToScriptHash(); //Owner Address

        public static bool Main(string operation, object[] args)
        {
            Storage.Put(Storage.CurrentContext, "Hello", "World");
            return true;
        }
    }
}
