using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System;
using System.ComponentModel;
using System.Numerics;

namespace BRC20
{
    public partial class BRC20 : SmartContract
    {
        #region Token Settings
        static readonly string Name = "RUSD";
        static readonly string Symbol = "RUSD";
        static readonly ulong Decimals = 2;
        static readonly ulong InitialSupply = 10000;        
        static readonly byte[] Owner = "AXAU9QQmB4cJvejnBGtugoQErRJzgzssG2".ToScriptHash();
        static readonly byte[] Token = new byte[] { 0x89, 0x77, 0x20, 0xd8, 0xcd, 0x76, 0xf4, 0xf0, 0x0a, 0xbf, 0xa3, 0x7c, 0x0e, 0xdd, 0x88, 0x9c, 0x20, 0x8f, 0xde, 0x9b };
        #endregion

        #region Notifications 
        [DisplayName("Issue")]
        public static event Action<byte[], BigInteger> OnIssue;
        [DisplayName("Approve")]
        public static event Action<byte[], byte[], BigInteger> OnApprove;
        [DisplayName("TransferFrom")]
        public static event Action<byte[], byte[], byte[], BigInteger> OnTransferFrom;
        [DisplayName("Transfer")]
        public static event Action<byte[], byte[], BigInteger> OnTransfer;
        #endregion

        #region Storage key prefixes
        static readonly string StoragePrefixContract = "11";
        static readonly string StoragePrefixBalance = "22";
        static readonly string StoragePrefixApprove = "33";
        #endregion

        public static object Main(string operation, object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                return Runtime.CheckWitness(Owner);
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                #region BRC20 METHODS
                if (operation == "name") return Name;
                if (operation == "symbol") return Symbol;
                if (operation == "decimals") return Decimals;
                if (operation == "totalSupply") return TotalSupply();
                if (operation == "balanceOf") return BalanceOf((byte[])args[0]);
                if (operation == "transfer") return Transfer((byte[])args[0], (byte[])args[1], (BigInteger)args[2]);
                if (operation == "issue") return Issue((byte[])args[0], (BigInteger)args[1]);
                if (operation == "approve") return Approve((byte[])args[0], (byte[])args[1], (BigInteger)args[2]);
                if (operation == "transferFrom") return TransferFrom((byte[])args[0], (byte[])args[1], (byte[])args[2], (BigInteger)args[3]);
                #endregion

                #region BRC10 METHODS
                if (operation == "supportedStandards") return new string[] { "BRC5", "BRC10", "BRC20" };
                #endregion

                #region ADMIN METHODS
                if (operation == "deploy") return Deploy();
                if (operation == "migrate") return Migrate(args);
                if (operation == "destroy") return Destroy();
                #endregion
            }
            return false;
        }
    }
}
