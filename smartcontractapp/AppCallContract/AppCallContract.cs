using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;

namespace AppCallContract
{
    public class AppCallContract : SmartContract
    {
        //静态调用
        [Appcall("b6b31246a8fd11d7cd12ad3958106a8be1175aae")]
        public static extern int Call(string operation);//参数与被调用合约保持一致即可

        //动态调用
        delegate object deleDyncall(string method, object[] args);//参数与被调用合约保持一致，需先设置或传入被调用合约

        public static object Main(string operation, object[] args)
        {
            if (operation == "staticCall")
            {
                return Call((string)args[0]);
            }
            if (operation == "setDyncallContract")
            {
                Storage.Put(Storage.CurrentContext, "dyncallContract", (byte[])args[0]);
                return true;
            }
            if (operation == "dyncallContract")
            {
                byte[] contract = Storage.Get(Storage.CurrentContext, "dyncallContract");
                deleDyncall dyncall = (deleDyncall)contract.ToDelegate();
                return dyncall((string)args[0], null);
            }
            return true;
        }
    }
}
