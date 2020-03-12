using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;

namespace SotrageContract
{
    public class SotrageContract : SmartContract
    {
        static readonly byte[] owner = "ATe3wDE9MPQXZuvhgPREdQNYkiCBF7JShY".ToScriptHash();//超级管理员地址
        static readonly string address = "ATe3wDE9MPQXZuvhgPREdQNYkiCBF7JShY";
        public static object Main(string operation)
        {
            if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == "put")
                {
                    if (!Runtime.CheckWitness(owner)) return false;
                    Storage.Put(Storage.CurrentContext, "lock", "hello");
                    return true;
                }
                else if (operation == "get")
                {
                    return Storage.Get(Storage.CurrentContext, "lock");
                }
                else if (operation == "delete")
                {
                    if (!Runtime.CheckWitness(owner)) return false;
                    Storage.Delete(Storage.CurrentContext, "lock");
                    return true;
                }
                else if (operation == "putMap")
                {
                    if (!Runtime.CheckWitness(owner)) return false;
                    StorageMap contract = Storage.CurrentContext.CreateMap(address);
                    contract.Put("mymap", "aaaaaaaaaaa");
                    //return contract.Get("mymap");
                    return true;
                }
                else if (operation == "getMap")
                {
                    return Storage.Get(Storage.CurrentContext, "ATe3wDE9MPQXZuvhgPREdQNYkiCBF7JShY\u0000mymap");//gas0.149
                }
                else if (operation == "getMap2")
                {
                    return Storage.Get(Storage.CurrentContext, address + "\u0000mymap");//gas0.154
                }
                else if (operation == "getMap3")
                {
                    return Storage.Get(Storage.CurrentContext, "ATe3wDE9MPQXZuvhgPREdQNYkiCBF7JShY\x00mymap");//gas0.157
                }
                else if (operation == "getMap4")
                {
                    return Storage.Get(Storage.CurrentContext, address + "\x00mymap");//gas0.157
                }
                else if (operation == "getMap5")
                {
                    return Storage.Get(Storage.CurrentContext, address);//return null
                }
            }
            return false;
        }
    }//end of class

    //Main(uint timestamp, string operation)
    //sb.EmitPush("");
    //sb.EmitPush(timestamp);
    //sb.EmitAppCall(UInt160.Parse("0x9fb64bfa3d30cff5ba006b5189d4c2a6da0180b6"));

    //Main(uint timestamp, string operation, object[] args)
    //sb.EmitPush(0);//array count
    //sb.Emit(OpCode.PACK);//array end opcode
    //sb.EmitPush("");
    //sb.EmitPush(timestamp);
    //sb.EmitAppCall(UInt160.Parse("0xfbd64900f1ae107d4240b60fa35ab563b03b722b"));
}
