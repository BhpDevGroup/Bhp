using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;

namespace BRC20
{
    public partial class BRC20 : SmartContract
    {
        /// <summary>
        /// 地址验证
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>true:有效地址, false:无效地址</returns>
        private static bool ValidateAddress(byte[] address)
        {
            if (address.Length != 20)
                return false;
            if (address.ToBigInteger() == 0)
                return false;
            return true;
        }

        /// <summary>
        /// 支付验证
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>true:可支付, false:不可支付</returns>
        private static bool IsPayable(byte[] address)
        {
            var c = Blockchain.GetContract(address);
            return c == null || c.IsPayable;
        }
    }
}
