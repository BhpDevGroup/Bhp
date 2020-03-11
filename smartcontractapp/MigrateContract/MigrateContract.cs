using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using Bhp.SmartContract.Framework.Services.System;

namespace MigrateContract
{
    public class MigrateContract : SmartContract
    {
        private static readonly byte[] Owner = "ATe3wDE9MPQXZuvhgPREdQNYkiCBF7JShY".ToScriptHash(); //Owner Address
        public static object Main(string operation, object[] args)
        {
            if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == "put")
                {
                    if (!Runtime.CheckWitness(Owner)) return false;
                    Storage.Put(Storage.CurrentContext, "migrate", (string)args[0]);
                    return true;
                }
                else if (operation == "get")
                {
                    return Storage.Get(Storage.CurrentContext, "migrate");
                }
                else if (operation == "migrate")
                {
                    if (!Runtime.CheckWitness(Owner))
                        return false;

                    if (args.Length < 9) return false;

                    byte[] script = Blockchain.GetContract(ExecutionEngine.ExecutingScriptHash).Script;
                    byte[] new_script = (byte[])args[0];
                    if (script == new_script)
                        return false;

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
            }
            return false;
        }

        private static bool Migrate(byte[] script, byte[] plist, byte rtype, ContractPropertyState cps, string name, string version, string author, string email, string description)
        {
            var contract = Contract.Migrate(script, plist, rtype, cps, name, version, author, email, description);
            return true;
        }
    }
}
