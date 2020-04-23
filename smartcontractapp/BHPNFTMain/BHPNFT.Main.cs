using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System;
using System.Numerics;

namespace BhpHashPowerNFT
{
    public partial class HashPowerContract : SmartContract
    {
        public static object Main(string operation, object[] args)
        {
            //UTXO转账转入转出都不允许
            if (Runtime.Trigger == TriggerType.Verification || Runtime.Trigger == TriggerType.VerificationR)
            {
                return false;
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                #region 基础方法
                //无入参只读类 
                if (operation == "name")
                {
                    return Name();
                }
                if (operation == "symbol")
                {
                    return Symbol();
                }
                if (operation == "decimals")
                {
                    return Decimals();
                }
                if (operation == "supportedStandards")
                {
                    return SupportedStandards();
                }
                #endregion

                #region 转账
                if (operation == "transfer")
                {
                    if (args.Length != 3) return false;
                    return Transfer((byte[])args[0], (BigInteger)args[1], (BigInteger)args[2]);
                }
                if (operation == "transferFrom")
                {
                    if (args.Length != 4) return false;
                    return TransferFrom((byte[])args[0],(byte[])args[1],(BigInteger)args[2], (BigInteger)args[3]);
                }
                #endregion

                #region 授权
                if (operation == "approve")
                {
                    if (args.Length != 2 && args.Length != 3) return false;
                    if (args.Length == 2) return Approve((byte[])args[0], (BigInteger)args[1], false);
                    return Approve((byte[])args[0], (BigInteger)args[1], (bool)args[2]);
                }
                if (operation == "approveAll")
                {
                    if (args.Length != 2) return false;
                    return SetApprovalForAll((byte[])args[0],(byte[])args[1]);
                }
                if (operation == "removeApproveAll")
                {
                    if (args.Length != 2) return false;
                    return RemoveApprovalForAll((byte[])args[0], (byte[])args[1]);
                }
                if (operation == "getApproveAllAddrs")
                {
                    if (args.Length != 1) return false;
                    return GetApproveAllAddrs((byte[])args[0]);
                }
                if (operation == "getApproveAsset")
                {
                    if (args.Length != 2) return false;
                    return GetApproveAsset((byte[])args[0],(byte[])args[1]);
                }
                #endregion

                #region 质押、解质押

                if (operation == "pledger")
                {
                    if (args.Length != 4) return false;
                    return Pledge((byte[])args[0], (byte[])args[1],(BigInteger)args[2], (bool)args[3]);
                }
                if (operation == "unpledger")
                {
                    if (args.Length != 1) return false;
                    return UnPledge((BigInteger)args[0]);
                }

                #endregion

                #region 查询
                
                if (operation == "getAsset")
                {
                    if (args.Length != 1) return false;
                    return GetAssetByAssetId((BigInteger)args[0]);
                }
                //地址下有效算力和
                if (operation == "balanceOf")
                {
                    if (args.Length != 1) return false;
                    return BalanceOf((byte[])args[0]);
                }
                if (operation == "assetIdsOfOwner")
                {
                    if (args.Length != 1) return false;
                    return GetOwnerNFTListByAddr((byte[])args[0]);
                }
                if (operation == "assetIdsOfIssuerKey")
                {
                    if (args.Length != 1) return false;
                    return GetIssuerKeyNFTListByIssuerKey((string)args[0]);
                }
                if (operation == "assetIdsOfPledger")
                {
                    if (args.Length != 1) return false;
                    return GetPledgerNFTListByAddr((byte[])args[0]);
                }
                #endregion

                #region 铸币
                //代币合约所有者操作(superAdmin)
                if (operation == "mintAsset")
                {
                    if (args.Length != 15) return false;
                    return IssueAsset((string)args[0],(byte[])args[1], (BigInteger)args[2], (BigInteger)args[3],
                        (BigInteger)args[4], (BigInteger)args[5], (BigInteger)args[6], (BigInteger)args[7], (BigInteger)args[8],
                        (BigInteger)args[9], (BigInteger)args[10], (BigInteger)args[11], (BigInteger)args[12], (BigInteger)args[13], (BigInteger)args[14]);
                }
                #endregion

                #region 修改NFT属性

                if (operation == "modifyNFTattribute")
                {
                    if (args.Length != 2) return false;
                    return ModifyNFTAttribute((BigInteger)args[0], (Map<string, object>)args[1]);
                }
                #endregion

                #region 合约升级

                if (operation == "migrate")
                {
                    return MigrateContract(args);
                }

                #endregion

                #region 授权地址操作

                //获取发行地址
                if (operation == "getIssuer")
                {
                    return GetIssuerAddrs();
                }
                //增加发行地址（仅超级管理员）
                if (operation == "addIssuer")
                {
                    if (args.Length != 2) return false;
                    return AddIssuerKeyAndIssuer((string)args[0],(byte[])args[1]);
                }
                //删除发行地址（仅超级管理员）
                if (operation == "removeIssuer")
                {
                    if (args.Length != 2) return false;
                    return RemoveIssuerKeyAndIssuer((string)args[0],(byte[])args[1]);
                }
                //判断地址是否为发行地址
                if (operation == "isMintAddr")
                {
                    if (args.Length != 1) return false;
                    return IsMintAddr((byte[])args[0]);
                }

                #endregion

                #region 资产分拆、合并、销毁

                //资产拆分
                if (operation == "splitAsset")
                {
                    if (args.Length != 4) return false;
                    return SplitAsset((byte[])args[0], (BigInteger)args[1], (BigInteger)args[2], (BigInteger)args[3]);
                }
                //资产合并
                if (operation == "mergeAsset")
                {
                    if (args.Length != 3) return false;
                    return MergeAsset((byte[])args[0], (BigInteger)args[1], (BigInteger)args[2]);
                }
                if (operation == "destoreyAsset")
                {
                    return DestoreyAsset((BigInteger)args[0]);
                }

                #endregion

            }

            return false;
        }
    }
}

