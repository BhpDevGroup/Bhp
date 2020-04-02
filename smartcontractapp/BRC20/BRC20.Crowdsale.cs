using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.System;
using System.Numerics;

namespace BRC20
{
    public partial class BRC20 : SmartContract
    {
        private static BigInteger GetTransactionAmount(object state)
        {
            var notification = (object[])state;
            // Checks notification format
            if (notification.Length != 4) return 0;
            // Only allow Transfer notifications
            if ((string)notification[0] != "Transfer") return 0;
            // Check dest
            if ((byte[])notification[2] != ExecutionEngine.ExecutingScriptHash) return 0;
            // Amount
            var amount = (BigInteger)notification[3];
            if (amount < 0) return 0;
            return amount;
        }
    }
}
