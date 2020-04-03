using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using Bhp.SmartContract.Framework.Services.System;
using System;

namespace BRC20
{
    public partial class BRC20 : SmartContract
    {
        private static bool Deploy()
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            StorageMap contract = Storage.CurrentContext.CreateMap(StoragePrefixContract);
            if (contract.Get("totalSupply") != null)
                throw new Exception("Contract already deployed");

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            balances.Put(Owner, InitialSupply);
            contract.Put("totalSupply", InitialSupply);

            OnTransfer(null, Owner, InitialSupply);
            return true;
        }

        public static bool Migrate(object[] args)
        {
            if (!Runtime.CheckWitness(Owner))
                return false;

            if (args.Length < 9) return false;

            byte[] script = Blockchain.GetContract(ExecutionEngine.ExecutingScriptHash).Script;
            byte[] new_script = (byte[])args[0];

            if (new_script.Length == 0) return false;

            if (script == new_script) return false;

            byte[] parameter_list = (byte[])args[1];
            byte return_type = (byte)args[2];
            ContractPropertyState cps = (ContractPropertyState)args[3];
            string name = (string)args[4];
            string version = (string)args[5];
            string author = (string)args[6];
            string email = (string)args[7];
            string description = (string)args[8];
            return Migrate(new_script, parameter_list, return_type, cps, name, version, author, email, description);
        }
        private static bool Migrate(byte[] script, byte[] plist, byte rtype, ContractPropertyState cps, string name, string version, string author, string email, string description)
        {
            var contract = Contract.Migrate(script, plist, rtype, cps, name, version, author, email, description);
            return true;
        }

        public static bool Destroy()
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            Contract.Destroy();
            return true;
        }
    }
}
