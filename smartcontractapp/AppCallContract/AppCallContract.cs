using Bhp.SmartContract.Framework;

namespace AppCallContract
{
    public class AppCallContract : SmartContract
    {
        [Appcall("b6b31246a8fd11d7cd12ad3958106a8be1175aae")]
        public static extern int Call(string operation);
        public static object Main()
        {
            return Call("get");
        }
    }
}
