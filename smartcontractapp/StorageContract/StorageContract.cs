using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;

namespace SotrageContract
{
    public class SotrageContract : SmartContract
    {
        static readonly byte[] superAdmin = Bhp.SmartContract.Framework.Helper.ToScriptHash("AXo6nRuiFxLqS9XnYS8x1f25eM5mGxkAq7");//超级管理员地址
        public static object Main(uint timestamp, string operation)
        {
            if (Runtime.Trigger == TriggerType.Verification)//取钱才会涉及这里
            {
                Header header = Blockchain.GetHeader(Blockchain.GetHeight());
                if (header.Timestamp < timestamp && Storage.Get(Storage.CurrentContext, "lock") == null)
                    return false;
                return true;
            }
            else if (Runtime.Trigger == TriggerType.VerificationR)//取钱才会涉及这里
            {
                return false;
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == "put")
                {
                    if (!Runtime.CheckWitness(superAdmin)) return false;
                    Storage.Put(Storage.CurrentContext, "lock", "hello");
                    return true;
                }
                else if (operation == "get")
                {
                    return Storage.Get(Storage.CurrentContext, "lock");
                }
                else if (operation == "delete")
                {
                    if (!Runtime.CheckWitness(superAdmin)) return false;
                    Storage.Delete(Storage.CurrentContext, "lock");
                    return true;
                }
                //if (operation == "upgrade")//合约的升级就是在合约中要添加这段代码来实现
                //{
                //    //不是管理员 不能操作
                //    if (!Runtime.CheckWitness(superAdmin))
                //        return false;

                //    if (args.Length != 1 && args.Length != 9)
                //        return false;

                //    byte[] script = Blockchain.GetContract(ExecutionEngine.ExecutingScriptHash).Script;
                //    byte[] new_script = (byte[])args[0];
                //    //如果传入的脚本一样 不继续操作
                //    if (script == new_script)
                //        return false;

                //    byte[] parameter_list = new byte[] { 0x07, 0x10 };
                //    byte return_type = 0x05;
                //    bool need_storage = (bool)(object)05;
                //    string name = "test";
                //    string version = "1.1";
                //    string author = "NEL";
                //    string email = "0";
                //    string description = "test";

                //    if (args.Length == 9)
                //    {
                //        parameter_list = (byte[])args[1];
                //        return_type = (byte)args[2];
                //        need_storage = (bool)args[3];
                //        name = (string)args[4];
                //        version = (string)args[5];
                //        author = (string)args[6];
                //        email = (string)args[7];
                //        description = (string)args[8];
                //    }
                //    Contract.Migrate(new_script, parameter_list, return_type, (ContractPropertyState)(05), name, version, author, email, description);
                //    return true;
                //}
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
