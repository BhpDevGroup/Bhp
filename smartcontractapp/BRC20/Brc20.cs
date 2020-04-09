using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System;
using System.ComponentModel;
using System.Numerics;

namespace BRC20
{
    public partial class BRC20 : SmartContract
    {
        #region Asset Settings
        static readonly string Name = "RUSD";//名称
        static readonly string Symbol = "RUSD";//简称
        static readonly ulong Decimals = 2;//精度
        static readonly ulong InitialSupply = 0;//初始化资产金额
        static readonly byte[] Owner = "ATe3wDE9MPQXZuvhgPREdQNYkiCBF7JShY".ToScriptHash();//管理员地址
        #endregion

        #region Notifications
        /// <summary>
        /// 铸币事件
        /// </summary>
        [DisplayName("Mint")]
        public static event Action<byte[], BigInteger> OnMint;
        /// <summary>
        /// 授权事件
        /// </summary>
        [DisplayName("Approve")]
        public static event Action<byte[], byte[], BigInteger> OnApprove;
        /// <summary>
        /// 被授权者转账事件
        /// </summary>
        [DisplayName("TransferFrom")]
        public static event Action<byte[], byte[], byte[], BigInteger> OnTransferFrom;
        /// <summary>
        /// 转账事件
        /// </summary>
        [DisplayName("Transfer")]
        public static event Action<byte[], byte[], BigInteger> OnTransfer;
        /// <summary>
        /// 资产销毁事件
        /// </summary>
        [DisplayName("DestroyAsset")]
        public static event Action<byte[]> OnDestroyAsset;
        #endregion

        #region Storage key prefixes
        static readonly string StoragePrefixContract = "contract";//合约
        static readonly string StoragePrefixBalance = "balance";//资产
        static readonly string StoragePrefixApprove = "approve";//授权
        static readonly string StoragePrefixMintAddr = "mint";//铸币
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
                #endregion

                #region Asset METHODS
                if (operation == "mint") return Mint((byte[])args[0], (BigInteger)args[1]);
                if (operation == "approve") return Approve((byte[])args[0], (byte[])args[1], (BigInteger)args[2]);
                if (operation == "transferFrom") return TransferFrom((byte[])args[0], (byte[])args[1], (byte[])args[2], (BigInteger)args[3]);
                if (operation == "destroyAsset") return DestroyAsset((byte[])args[0]);
                #endregion

                #region BRC10 METHODS
                if (operation == "supportedStandards") return new string[] { "BRC5", "BRC10", "BRC20" };
                #endregion

                #region ADMIN METHODS
                if (operation == "deploy") return Deploy();
                if (operation == "migrate") return Migrate(args);
                if (operation == "destroy") return Destroy();
                if (operation == "setMintAddr") return SetMintAddr((byte[])args[0]);
                #endregion
            }
            return false;
        }
    }
}
