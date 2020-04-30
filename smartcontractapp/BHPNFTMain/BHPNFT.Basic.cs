using Bhp.SmartContract.Framework;
using System.Numerics;
using Bhp.SmartContract.Framework.Services.Bhp;
using Helper = Bhp.SmartContract.Framework.Helper;
using System;

namespace BhpHashPowerNFT
{
    public partial class HashPowerContract : SmartContract
    {
        
        #region 基础方法 (对外)
        public static string Name()
        {
            return "BTCT";
        }

        public static string Symbol()
        {
            return "BTCT";
        }

        public static BigInteger Decimals()
        {
            return 8;
        }

        public static string[] SupportedStandards()
        {
            return new string[] { "BAS101", "BAS201" };
        }

        /// <summary>
        /// totalSupply作为asset自增id
        /// </summary>
        /// <returns></returns>
        public static BigInteger TotalSupply()
        {
            StorageMap contractState = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
            return contractState.Get(TotalSupplyMapKey).AsBigInteger();
        }

        #endregion

        #region 基础查询

        /// <summary>
        /// 通过资产id查询资产信息
        /// </summary>
        /// <param name="assetId">资产Id</param>
        /// <returns></returns>
        public static Asset GetAssetByAssetId(BigInteger assetId)
        {
            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);

            var data = assetMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                Asset asset = Helper.Deserialize(data) as Asset;
                return asset;
            }
            return new Asset();
        }

        /// <summary>
        /// 获取代币所有权地址
        /// </summary>
        /// <param name="assetId>资产id</param>
        /// <returns></returns>
        public static byte[] getOwnerByAssetId(BigInteger assetId)
        {
            return GetAssetByAssetId(assetId).owner;
        }

        #endregion

        #region 所有权地址、质押地址、发行地址key 资产索引操作 (不对外)

        /// <summary>
        /// 增加ower地址nft资产索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        private static bool AddOwnerNFTList(byte[] addr, BigInteger assetId)
        {
            Storage.Put(StoragePrefixOwnerNFTList.AsByteArray().Concat(addr).Concat(assetId.AsByteArray()), assetId);
            return true;
        }

        /// <summary>
        /// 增加pledger地址nft资索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        private static bool AddPledgerNFTList(byte[] pledger, BigInteger assetId)
        {
            Storage.Put(StoragePrefixPledgerNFTList.AsByteArray().Concat(pledger).Concat(assetId.AsByteArray()), assetId);
            return true;
        }

        /// <summary>
        /// 增加issuerKey 的nft资索引
        /// </summary>
        /// <param name="issuerKey">发行地址key</param>
        /// <param name="assetId">资产id</param>
        private static bool AddIssuerKeyNFTList(string issuerKey, BigInteger assetId)
        {
            Storage.Put(StoragePrefixIssuerKeyNFTList.AsByteArray().Concat(issuerKey.AsByteArray()).Concat(assetId.AsByteArray()), assetId);
            return true;
        }

        /// <summary>
        /// 删除owner地址拥有的nft资产索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        private static bool RemoveOwnerNFTList(byte[] addr, BigInteger assetId)
        {
            Storage.Delete(StoragePrefixOwnerNFTList.AsByteArray().Concat(addr).Concat(assetId.AsByteArray()));
            return true;
        }

        /// <summary>
        /// 删除pledger地址拥有的nft资产索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        private static bool RemovePledgerNFTList(byte[] addr, BigInteger assetId)
        {
            Storage.Delete(StoragePrefixPledgerNFTList.AsByteArray().Concat(addr).Concat(assetId.AsByteArray()));
            return true;
        }

        /// <summary>
        /// 删除issuerKey拥有的nft资产索引
        /// </summary>
        /// <param name="issuerKey">发行地址key</param>
        /// <param name="assetId">资产id</param>
        private static bool RemoveIssuerKeyNFTList(string issuerKey, BigInteger assetId)
        {
            Storage.Delete(StoragePrefixIssuerKeyNFTList.AsByteArray().Concat(issuerKey.AsByteArray()).Concat(assetId.AsByteArray()));
            return true;
        }

        #endregion

        #region 授权发行地址增、减、查询, 判断地址是否为授权发行地址（对外）

        /// <summary>
        /// 增加授权发行地址(仅超级管理员可操作)
        /// </summary>
        /// <param name="addr">授权发行地址</param>
        /// <returns></returns>
        private static bool AddIssuer(byte[] addr)
        {
            Storage.Put(StoragePrefixIssuerAddrs.AsByteArray().Concat(addr), addr);
            return true;
        }

        /// <summary>
        /// 删除授权发行地址(仅超级管理员可操作)
        /// </summary>
        /// <param name="addr">授权发行地址</param>
        /// <returns></returns>
        private static bool RemoveIssuer(byte[] addr)
        {
            Storage.Delete(StoragePrefixIssuerAddrs.AsByteArray().Concat(addr));
            return true;
        }

        /// <summary>
        /// 判断地址是否在授权发行地址
        /// </summary>
        /// <param name="addr">地址</param>
        /// <returns></returns>
        public static bool IsMintAddr(byte[] addr)
        {
            if (Storage.Get(StoragePrefixIssuerAddrs.AsByteArray().Concat(addr)) == null)
            {
                return false;
            }
            return true;
        }

        //发行地址key-address增减
        private static bool AddIssuerKey(string issuerKey, byte[] addr)
        {
            if (issuerKey.Length < 1) return false;
            if (!ValidateAddress(addr)) return false;
            if (!Runtime.CheckWitness(superAdmin)) return false;
            Storage.Put(StoragePrefixIssuerKey.AsByteArray().Concat(issuerKey.AsByteArray()), addr);
            return true;
        }

        /// <summary>
        /// 删除发行地址key-address
        /// </summary>
        /// <param name="issuerKey"></param>
        /// <returns></returns>
        private static bool RemoveIssuerKey(string issuerKey)
        {
            Storage.Delete(StoragePrefixIssuerKey.AsByteArray().Concat(issuerKey.AsByteArray()));
            return true;
        }

        /// <summary>
        /// 通过issuerKey获取issuer
        /// </summary>
        /// <param name="issuerKey"></param>
        /// <returns></returns>
        public static byte[] GetIssuerByKey(string issuerKey)
        {
           return Storage.Get(StoragePrefixIssuerKey.AsByteArray().Concat(issuerKey.AsByteArray()));
        }

        /// <summary>
        /// 获取所有的发行地址及发行地址key
        /// </summary>
        /// <returns></returns>
        public static Iterator<byte[], byte[]> GetIssuerKeysAddress()
        {
            return Storage.Find(StoragePrefixIssuerKey.AsByteArray());
        }

        #endregion

        #region  授权 （对外）

        /// <summary>
        /// 部分授权
        /// </summary>
        /// <param name="spender">授权地址</param>
        /// <param name="assetId">资产id</param>
        /// <param name="revoke">revoke = true，取消授权; revoke = fasle,授权</param>
        /// <returns></returns>
        public static bool Approve(byte[] spender, BigInteger assetId, bool revoke)
        {
            if (!ValidateAddress(spender)) return false;
            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var assetData = assetMap.Get(assetId.AsByteArray());
            if (assetData.Length > 0)
            {
                Asset asset = Helper.Deserialize(assetData) as Asset;
                //判断是否为所有权地址在操作
                if (!Runtime.CheckWitness(asset.owner)) return false;
                if (!revoke) //授权
                {
                    Storage.Put(StoragePrefixApproveAsset.AsByteArray().Concat(asset.owner).Concat(spender).Concat(assetId.AsByteArray()), assetId);
                    onApprove(asset.owner,spender,1);
                    onNFTApprove(asset.owner, spender, assetId);
                }
                //取消授权
                else
                {
                    Storage.Delete(StoragePrefixApproveAsset.AsByteArray().Concat(asset.owner).Concat(spender).Concat(assetId.AsByteArray()));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除部分资产授权中该资产授权，（此方法内部调用，不判断权限）
        /// </summary>
        /// <param name="approve"></param>
        /// <param name="assetId"></param>
        /// <param name="revoke"></param>
        /// <returns></returns>
        private static void RemoveApproveForAsset(BigInteger assetId)
        {
            byte[] ApproveAssetPrefix = StoragePrefixApproveAsset.AsByteArray();
            Iterator<byte[], byte[]> approveAssetData = Storage.Find(ApproveAssetPrefix);
            while (approveAssetData.Next())
            {
                byte[] allKey = approveAssetData.Key;
                if (allKey.Length < (ApproveAssetPrefix.Length + 40)) throw new FormatException("allKey.Length is error");
                int assetKeyLength = allKey.Length - ApproveAssetPrefix.Length - 40;
                byte[] assetKey = allKey.Last(assetKeyLength);
                if (assetKey == assetId.AsByteArray())
                {
                    Storage.Delete(allKey);
                }
            }
        }

        //授权某地址操作账户里所有资产
        //授权的操作包括转让、分拆、合并、质押
        public static bool SetApprovalForAll(byte[] owner, byte[] spender)
        {
            if (!ValidateAddress(owner) || !ValidateAddress(spender)) return false;
            if (!Runtime.CheckWitness(owner)) return false;
            Storage.Put(StoragePrefixApproveAll.AsByteArray().Concat(owner).Concat(spender), spender);
            return true;
        }

        //删除全部授权
        public static bool RemoveApprovalForAll(byte[] owner, byte[] spender)
        {
            if (!ValidateAddress(owner) || !ValidateAddress(spender)) return false;
            if (!Runtime.CheckWitness(owner)) return false;
            Storage.Delete(StoragePrefixApproveAll.AsByteArray().Concat(owner).Concat(spender));
            return true;
        }

        /// <summary>
        /// 查询owner 地址全部授权了那些地址
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static Iterator<byte[], byte[]> GetApproveAllAddrs(byte[] owner)
        {
            return Storage.Find(StoragePrefixApproveAll.AsByteArray().Concat(owner));
        }

        /// <summary>
        /// 查询owner 地址给 spender 地址授权了那些资产
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="spender"></param>
        /// <returns></returns>
        public static Iterator<byte[], byte[]> GetApproveAsset(byte[] owner, byte[] spender)
        {
            return Storage.Find(StoragePrefixApproveAsset.AsByteArray().Concat(owner).Concat(spender));
        }
        #endregion


        #region 基础判断（不对外）

        /// <summary>
        /// 判断是否为有效的算力资产
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        private static bool IsEffectiveHashPower(Asset asset)
        {
            if (asset.assetState == 0)
            {
                return false;
            }
            if (IsExpire(asset))//已过期 
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断是否过期
        /// </summary>
        /// <param name="asset">资产</param>
        /// <returns></returns>
        private static bool IsExpire(Asset asset)
        {
            BigInteger currentDate = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp;
            BigInteger basicHashPowerExpiryDate = asset.basicHashPowerExpiryDate;
            if (currentDate >= basicHashPowerExpiryDate) 
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否销毁
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        private static bool IsDestorey(Asset asset)
        {
            if (asset.assetState == 0) 
            {
                return true;
            }
            return false;
        }
      
        /// <summary>
        /// 判断资产是否能转账，判断过期，及是否在锁定期，过期及锁定期内不能转账,判断是否销毁
        /// </summary>
        /// <param name="assetId">资产id</param>
        /// <returns></returns>
        private static bool IsTransfer(Asset asset)
        {
            if (IsExpire(asset))
            {
                return false;
            }
            BigInteger currentDate = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp;
            BigInteger unLockDate = asset.unLockDate;
            if (currentDate <= unLockDate)
            {
                return false;
            }
            if (IsDestorey(asset))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 是否被质押
        /// </summary>
        /// <param name="assetId">资产id</param>
        /// <returns></returns>
        private static bool IsPledger(Asset asset)
        {
            if (asset.assetState == 3)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否授权了资产
        /// </summary>
        /// <param name="owner">所有权地址</param>
        /// <param name="spender">授权地址</param>
        /// <param name="assetId">资产id</param>
        /// <returns></returns>
        private static bool IsApprove(byte[] owner, byte[] spender, BigInteger assetId)
        {
            //判断是否授权了所有资产
            var approveAddr = Storage.Get(StoragePrefixApproveAll.AsByteArray().Concat(owner).Concat(spender));
            if (approveAddr != null)
            {
                return true;
            }
            //判断部分授权中是否授权了该资产
            var approveAssetId = Storage.Get(StoragePrefixApproveAsset.AsByteArray().Concat(owner).Concat(spender).Concat(assetId.AsByteArray()));
            if (approveAssetId != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否有权限操作资产
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="spender"></param>
        /// <param name="assetId"></param>
        /// <returns></returns>
        private static bool JudgeJurisdiction(byte[] owner, byte[] spender, BigInteger assetId,byte[] callingScript)
        {
            //如果是owner地址，则可以操作
            if (Runtime.CheckWitness(owner) || callingScript.AsBigInteger() == owner.AsBigInteger())
            {
                return true;
            }
            //如果不是owner地址，必须是授权地址且签名通过才能操作
            else
            {
                if ( IsApprove(owner, spender, assetId) && (Runtime.CheckWitness(spender) || callingScript.AsBigInteger() == spender.AsBigInteger()))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 是否能值拆分
        /// </summary>
        /// <param name="owner">owner地址</param>
        /// <param name="addr">质押拆分为质押地址，授权拆分为授权地址</param>
        /// <param name="asset"></param>
        /// <param name="callingScript"></param>
        /// <returns></returns>
        private static bool IsSplitAsset(byte[] owner, byte[] addr, Asset asset, byte[] callingScript) 
        {
            if (IsPledger(asset))
            {
                if (addr != asset.pledger) return false;
                if (Runtime.CheckWitness(addr) || callingScript.AsBigInteger() == asset.pledger.AsBigInteger())
                {
                    return true;
                }
            }
            else
            {
                if (asset.owner == addr)
                {
                    if (Runtime.CheckWitness(addr) || callingScript.AsBigInteger() == owner.AsBigInteger())
                    {
                        return true;
                    }
                }
                if (IsApprove(owner, addr, asset.assetId))
                {
                    if (Runtime.CheckWitness(addr) || callingScript.AsBigInteger() == addr.AsBigInteger()) 
                    {
                        return true;
                    }
                }
            }
            return false;
        }


    #endregion

    /// <summary>
    /// 查询所有的owner地址的所有资产
    /// </summary>
    /// <returns></returns>
    public static Iterator<byte[], byte[]> GetOwnerNFTListByAddr(byte[] addr)
        {
            return Storage.Find(StoragePrefixOwnerNFTList.AsByteArray().Concat(addr));
        }

        /// <summary>
        /// 查询所有的pledger地址的所有资产
        /// </summary>
        /// <returns></returns>
        public static Iterator<byte[], byte[]> GetPledgerNFTListByAddr(byte[] addr)
        {
            return Storage.Find(StoragePrefixPledgerNFTList.AsByteArray().Concat(addr));
        }
      
        /// <summary>
        /// 查询所有的issuerKey下的所有资产
        /// </summary>
        /// <returns></returns>
        public static Iterator<byte[], byte[]> GetIssuerKeyNFTListByIssuerKey(string issuerKey)
        {
            return Storage.Find(StoragePrefixIssuerKeyNFTList.AsByteArray().Concat(issuerKey.AsByteArray())  );
            //return Storage.Find(StoragePrefixIssuerKey.AsByteArray().Concat(addr));
        }

        /// <summary>
        /// 查询所有发行地址
        /// </summary>
        /// <returns></returns>
        public static Iterator<byte[], byte[]> GetIssuerAddrs()
        {
            return Storage.Find(StoragePrefixIssuerAddrs.AsByteArray());
        }

    }
}
