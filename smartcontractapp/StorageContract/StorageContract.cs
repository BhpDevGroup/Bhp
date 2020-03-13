using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using StorageContract;

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
                /****************** map *********************/
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
                /****************** object map *********************/
                else if (operation == "putOmap")
                {
                    //if (!Runtime.CheckWitness(owner)) return false;
                    BasicData basicData = new BasicData() { time = 1584002811, value = 123 };
                    BasicData advancedData = new BasicData() { time = 1584069212, value = 321 };
                    AddressData addressData = new AddressData { address = address, basic = new BasicData[] { basicData }, advanced = new BasicData[] { advancedData } };
                    ObjectMap objectMap = new ObjectMap() { addressData = new AddressData[] { addressData } };
                    Storage.Put(Storage.CurrentContext, "objMap", objectMap.Serialize());
                    return objectMap.Serialize();
                    //[{"type":"ByteArray","value":"800180018003002241546533774445394d5051585a7576686750524564514e596b69434246374a536859800181020004fbf6695e00017b8001810200045cfa6a5e00024101"}]
                    //return true;
                }
                else if (operation == "getOmap")
                {
                    byte[] obj = Storage.Get(Storage.CurrentContext, "objMap");
                    ObjectMap objectMap = ObjectMap.Deserialize(obj);
                    return objectMap;
                    //{"type":"Array","value":[{"type":"Array","value":[{"type":"Array","value":[{"type":"ByteArray","value":"41546533774445394d5051585a7576686750524564514e596b69434246374a536859"},{"type":"Array","value":[{"type":"Array","value":[{"type":"ByteArray","value":"fbf6695e"},{"type":"ByteArray","value":"7b"}]}]},{"type":"Array","value":[{"type":"Array","value":[{"type":"ByteArray","value":"5cfa6a5e"},{"type":"ByteArray","value":"4101"}]}]}]}]}]}]
                }
                else if (operation == "getOmap2")
                {
                    byte[] obj = Storage.Get(Storage.CurrentContext, "objMap");
                    ObjectMap objectMap = ObjectMap.Deserialize(obj);
                    AddressData addressData = AddressData.GetAddressData(objectMap, address);
                    return addressData;
                    //[{"type":"Array","value":[{"type":"ByteArray","value":"41546533774445394d5051585a7576686750524564514e596b69434246374a536859"},{"type":"Array","value":[{"type":"Array","value":[{"type":"ByteArray","value":"fbf6695e"},{"type":"ByteArray","value":"7b"}]}]},{"type":"Array","value":[{"type":"Array","value":[{"type":"ByteArray","value":"5cfa6a5e"},{"type":"ByteArray","value":"4101"}]}]}]}]
                }
                else if (operation == "getOmap3")
                {
                    byte[] obj = Storage.Get(Storage.CurrentContext, "objMap");
                    ObjectMap objectMap = ObjectMap.Deserialize(obj);
                    AddressData addressData = AddressData.GetAddressData(objectMap, address);
                    BasicData[] basics = AddressData.GetBasicData(addressData, AccountType.Basic);
                    return basics;
                    //[{"type":"Array","value":[{"type":"Array","value":[{"type":"ByteArray","value":"fbf6695e"},{"type":"ByteArray","value":"7b"}]}]}]
                }
                else if (operation == "getOmap4")
                {
                    byte[] obj = Storage.Get(Storage.CurrentContext, "objMap");
                    ObjectMap objectMap = ObjectMap.Deserialize(obj);
                    AddressData addressData = AddressData.GetAddressData(objectMap, address);
                    BasicData[] basics = AddressData.GetBasicData(addressData, AccountType.Basic);
                    BasicData data = BasicData.GetByTime(basics, 1584002811);
                    return data;
                    //[{"type":"Array","value":[{"type":"ByteArray","value":"fbf6695e"},{"type":"ByteArray","value":"7b"}]}]
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
