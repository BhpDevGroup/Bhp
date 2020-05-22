using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;

namespace RenewContract
{
    public class RenewContract : SmartContract
    {
        public static uint Main(string asset, uint year)
        {
            if (Runtime.Trigger == TriggerType.Application)
            {
                if (year <= 0) return 0;
                Asset ass = Blockchain.GetAsset(asset.AsByteArray());
                return ass.Renew((byte)year);
            }
            return 0;
        }
    }
}
