using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System;
using System.Linq;
using System.Numerics;
using Helper = Bhp.SmartContract.Framework.Helper;

namespace BhpHashPowerNFT
{
    public partial class HashPowerContract : SmartContract
    {

        #region 转账 （对外）

        //转账给他人，只能由owner调用，只能转让正常状态下的资产，支持拆分转让
        /// <summary>
        /// 所有权地址转账
        /// </summary>
        /// <param name="to">接收地址</param>
        /// <param name="assetId">资产id</param>
        /// <returns></returns>
        public static bool Transfer(byte[] to, BigInteger assetId, BigInteger amount,byte[] callingScript)
        {
            //判断地址是否合法
            if (!ValidateAddress(to)) return false;
            if (!IsPayable(to)) return false;

            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = assetMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                //旧的资产
                Asset oldAsset = Helper.Deserialize(data) as Asset;
                if (!Runtime.CheckWitness(oldAsset.owner) && callingScript.AsBigInteger() != oldAsset.owner.AsBigInteger()) return false;
                if (oldAsset.assetState != 1) return false;
                if (!IsTransfer(oldAsset)) return false; //判断是否能转账，过期及锁定期不能转账
                if (IsPledger(oldAsset)) return false; //判断是否被质押
                if (oldAsset.basicHashPowerAmount < amount || amount < 0) return false;
                if (oldAsset.owner == to) return true; //自己转给自己，不做操作
                if (oldAsset.basicHashPowerAmount == amount) //全部转出
                {
                    var addrFrom = oldAsset.owner;
                    oldAsset.owner = to;
                    assetMap.Put(assetId.AsByteArray(), Helper.Serialize(oldAsset));

                    //删除from 地址资产索引，增加to地址资产索引
                    RemoveOwnerNFTList(addrFrom, oldAsset.assetId);
                    //增加to地址拥有的资产id索引
                    AddOwnerNFTList(to, oldAsset.assetId);
                    //如果该资产已经授权，删除部分
                    RemoveApproveForAsset(oldAsset.assetId);

                    onTransfer(addrFrom, to, 1);
                    onNFTTransfer(addrFrom, to, assetId, amount);
                    return true;
                }
                //部分转让
                else
                {
                    Asset newAsset = TransferAsset(to, amount, assetMap, oldAsset);

                    //增加to地址资产索引
                    AddOwnerNFTList(to, newAsset.assetId);
                    //增加发行key地址索引
                    AddIssuerKeyNFTList(newAsset.issuerKey, newAsset.assetId);

                    onTransfer(oldAsset.owner, to, 1);
                    onNFTTransfer(oldAsset.owner, to, assetId, amount);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 授权地址转账，质押地址转账,
        /// </summary>
        /// <param name="from">授权地址或质押地址</param>
        /// <param name="to">接收地址</param>
        /// <param name="assetId">资产id</param>
        /// <returns></returns>
        public static bool TransferFrom(byte[] from, byte[] to, BigInteger assetId, BigInteger amount,byte[] callingScript)
        {
            //判断地址是否合法
            if (!ValidateAddress(from)) return false;
            if (!ValidateAddress(to)) return false;
            if (!IsPayable(to)) return false;

            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = assetMap.Get(assetId.AsByteArray());
            //token 存在
            if (data.Length > 0)
            {
                Asset oldAsset = Helper.Deserialize(data) as Asset;
                BigInteger assetState = oldAsset.assetState;
                if (!IsTransfer(oldAsset)) return false; //判断是否在锁定期或者已经过期
                if (oldAsset.basicHashPowerAmount < amount || amount < 0) return false; //金额不正确
                if (oldAsset.owner == to) return true; //转出地址等于转出地址，不做操作
                //授权地址只能转出资产状态为1，正常状态的资产
                if (assetState == 1)
                {
                    //判断from 是否为授权地址
                    if (!IsApprove(oldAsset.owner, from, assetId)) return false;
                    if (!Runtime.CheckWitness(from) && callingScript.AsBigInteger() != from.AsBigInteger()) return false;
                    if (oldAsset.basicHashPowerAmount == amount) //全部转出
                    {
                        var addrFrom = oldAsset.owner;
                        oldAsset.owner = to;
                        assetMap.Put(assetId.AsByteArray(), Helper.Serialize(oldAsset));

                        //删除owner地址资产索引
                        RemoveOwnerNFTList(addrFrom, oldAsset.assetId);
                        //增加to地址拥有的资产id索引
                        AddOwnerNFTList(to, oldAsset.assetId);
                        //如果该资产已经授权，删除资产所有的部分授权
                        RemoveApproveForAsset(oldAsset.assetId);

                        //触发事件
                        onTransfer(oldAsset.owner, to, 1);
                        onNFTTransfer(oldAsset.owner, to, assetId, amount);
                        return true;
                    }
                    else
                    {
                        Asset newAsset = TransferAsset(to, amount, assetMap, oldAsset);

                        // 增加to地址资产索引
                        AddOwnerNFTList(to, newAsset.assetId);
                        //增加发行key地址NFT索引
                        AddIssuerKeyNFTList(newAsset.issuerKey, newAsset.assetId);

                        //触发事件
                        onTransfer(oldAsset.owner, to, 1);
                        onNFTTransfer(oldAsset.owner, to, assetId, amount);
                        return true;
                    }
                }
                if (IsPledger(oldAsset))
                {
                    if (!Runtime.CheckWitness(oldAsset.pledger) && callingScript.AsBigInteger() != oldAsset.pledger.AsBigInteger()) return false;
                    if (oldAsset.basicHashPowerAmount == amount) //全部转出
                    {
                        byte[] addrFrom = oldAsset.owner;
                        byte[] oldPledger = oldAsset.pledger;
                        oldAsset.owner = to;
                        //转出后，质押地址置空
                        oldAsset.pledger = new byte[0];
                        assetMap.Put(assetId.AsByteArray(), Helper.Serialize(oldAsset));

                        //删除owner地址资产索引
                        RemoveOwnerNFTList(addrFrom, oldAsset.assetId);
                        //增加to地址拥有的资产id索引
                        AddOwnerNFTList(to, oldAsset.assetId);
                        //如果该资产已经授权，删除资产所有的部分授权
                        RemoveApproveForAsset(oldAsset.assetId);
                        //删除质押地址该资产索引
                        RemovePledgerNFTList(oldPledger, oldAsset.assetId);

                        //触发事件
                        onTransfer(oldAsset.owner, to, 1);
                        onNFTTransfer(oldAsset.owner, to, assetId, amount);
                        return true;
                    }
                    else
                    {
                        Asset newAsset = TransferAsset(to, amount, assetMap, oldAsset);

                        //增加to地址拥有的资产id索引
                        AddOwnerNFTList(to, oldAsset.assetId);
                        //增加发行key地址NFT索引
                        AddIssuerKeyNFTList(newAsset.issuerKey, newAsset.assetId);

                        //触发事件
                        onTransfer(oldAsset.owner, to, 1);
                        onNFTTransfer(oldAsset.owner, to, assetId, amount);
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        private static Asset TransferAsset(byte[] to, BigInteger amount, StorageMap assetMap, Asset oldAsset)
        {
            //计算新的余额资产
            BigInteger newFloatingHashPowerAmount = (oldAsset.floatingHashPowerAmount * amount) / oldAsset.basicHashPowerAmount;
            BigInteger newRegularHashPowerAmount = (oldAsset.regularHashPowerAmount * amount) / oldAsset.basicHashPowerAmount;
            BigInteger newUptoStdHashPowerAmount = (oldAsset.uptoStdHashPowerAmount * amount) / oldAsset.basicHashPowerAmount;

            //修改老资产
            oldAsset.basicHashPowerAmount = oldAsset.basicHashPowerAmount - amount;
            oldAsset.floatingHashPowerAmount = oldAsset.floatingHashPowerAmount - newFloatingHashPowerAmount;
            oldAsset.regularHashPowerAmount = oldAsset.regularHashPowerAmount - newRegularHashPowerAmount;
            oldAsset.uptoStdHashPowerAmount = oldAsset.uptoStdHashPowerAmount - newUptoStdHashPowerAmount;

            //创建新资产
            StorageMap contractStateMap = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
            BigInteger totalSupply = contractStateMap.Get(TotalSupplyMapKey).AsBigInteger();
            Asset newAsset = new Asset();
            newAsset.assetId = totalSupply + 1;
            newAsset.owner = to; //资产所有者为to地址
            newAsset.issuerKey = oldAsset.issuerKey; //发行地址key
            newAsset.pledger = new byte[0]; //质押人地址
            newAsset.ownershipStartDate = oldAsset.ownershipStartDate; //所有权开始日期
            newAsset.basicHashPowerAmount = amount; //基础算力
            newAsset.basicHashPowerExpiryDate = oldAsset.basicHashPowerExpiryDate; //基础算力有效日期
            newAsset.floatingHashPowerAmount = newFloatingHashPowerAmount; //活期上浮
            newAsset.floatingHashPowerExpiryDate = oldAsset.floatingHashPowerExpiryDate;
            newAsset.regularHashPowerAmount = newRegularHashPowerAmount; //定期上浮
            newAsset.regularHashPowerExpiryDate = oldAsset.regularHashPowerExpiryDate;
            newAsset.uptoStdHashPowerAmount = newUptoStdHashPowerAmount; //达标上浮
            newAsset.uptoStdHashPowerExpiryDate = oldAsset.uptoStdHashPowerExpiryDate;
            newAsset.unLockDate = oldAsset.unLockDate; //定期解锁时间
            newAsset.incomePercent = oldAsset.incomePercent; //分币比例
            newAsset.isIncomePledged = oldAsset.isIncomePledged; //是否质押分币权,如果是，则分币到质押地址
            newAsset.assetType = oldAsset.assetType; //算力类型，1，单挖；2,双挖; 3，高波
            newAsset.assetState = oldAsset.assetState; //Normal, //1：正常状态，可转让、可质押

            contractStateMap.Put(TotalSupplyMapKey, newAsset.assetId); //回写资产id

            //更新存储区资产信息
            assetMap.Put(newAsset.assetId.AsByteArray(), Helper.Serialize(newAsset));
            assetMap.Put(oldAsset.assetId.AsByteArray(), Helper.Serialize(oldAsset));
            return newAsset;
        }

        #endregion

        #region 质押、解质押 （对外）

        /// <summary>
        ///  质押资产
        /// </summary>
        /// <param name="addr">质押地址</param>
        /// <param name="assetId">资产id</param>
        /// <param name="isIncomePledged">是否质押分币权</param>
        /// <returns></returns>
        public static bool Pledge(byte[] addr, byte[] pledger, BigInteger assetId, bool isIncomePledged,byte[] callingScript)
        {
            if (!ValidateAddress(pledger)) return false;

            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = assetMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                Asset asset = Helper.Deserialize(data) as Asset;
                //锁定期可以质押资产，只需要判断是否过期及是否销毁
                if (IsExpire(asset)) return false;
                if (IsDestorey(asset)) return false;
                if (!JudgeJurisdiction(asset.owner, addr, asset.assetId, callingScript)) return false;

                asset.pledger = pledger;
                asset.assetState = 3; //质押状态
                asset.isIncomePledged = isIncomePledged;

                //增加质押地址所拥有资产索引
                AddPledgerNFTList(pledger, assetId);

                assetMap.Put(assetId.AsByteArray(), Helper.Serialize(asset));

                onPledged(asset.owner, asset.pledger, 1);
                onNFTPledged(asset.owner, asset.pledger, assetId);
                return true;
            }
            return false;
        }

        //解除质押
        public static bool UnPledge(BigInteger assetId)
        {

            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = assetMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                Asset asset = Helper.Deserialize(data) as Asset;
                byte[] pledgerAddr = asset.pledger;
                if (!Runtime.CheckWitness(asset.pledger)) return false;

                asset.pledger = new byte[0]; //质押地址置空
                asset.isIncomePledged = false;
                asset.assetState = 1; //质押状态改为正常

                assetMap.Put(assetId.AsByteArray(), Helper.Serialize(asset));

                //删除质押地址的资产索引
                RemovePledgerNFTList(pledgerAddr, assetId);

                onPledged(asset.owner, asset.pledger, 1);
                onNFTPledged(asset.owner, asset.pledger, assetId);
                return true;
            }
            return false;
        }

        #endregion

        #region 资产分拆、合并

        //分拆资产，由owner或owner授权人发起
        //生成一份基础算力为newBalance的新的资产，原资产基础算力调整为remainBalance，其他算力按比例分割
        //返回新生成资产的assetID
        /// <summary>
        ///资产分拆 
        /// </summary>
        /// <param name="assetId">分拆的资产id</param>
        /// <param name="remainBalance">分拆后剩余算力</param>
        /// <param name="newBalance">新资产基础算力</param>
        /// <returns></returns>
        public static BigInteger SplitAsset(byte[] addr, BigInteger assetId, BigInteger remainBalance, BigInteger newBalance, byte[] callingScript)
        {
            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = assetMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                //旧的资产
                Asset oldAsset = Helper.Deserialize(data) as Asset;
                //返回0表示资产分拆失败
                if (!JudgeJurisdiction(oldAsset.owner, addr, assetId, callingScript)) return 0;
                if (!IsTransfer(oldAsset)) return 0;//判断是否过期，是否在锁定期
                if (IsPledger(oldAsset)) return 0; //质押资产不可分拆
                if (oldAsset.assetType != 1) return 0;
                if (remainBalance < 0 || newBalance < 0) return 0;
                if (remainBalance == 0 || newBalance == 0) return oldAsset.assetId;
                if ((remainBalance + newBalance) != oldAsset.basicHashPowerAmount) return 0; //分拆数量不正确

                BigInteger newFloatingHashPowerAmount = (newBalance * oldAsset.floatingHashPowerAmount) / oldAsset.basicHashPowerAmount;
                BigInteger newRegularHashPowerAmount = (newBalance * oldAsset.regularHashPowerAmount) / oldAsset.basicHashPowerAmount;
                BigInteger newUptoStdHashPowerAmount = (newBalance * oldAsset.uptoStdHashPowerAmount) / oldAsset.basicHashPowerAmount;

                //修改老资产
                oldAsset.basicHashPowerAmount = remainBalance;
                oldAsset.floatingHashPowerAmount = oldAsset.floatingHashPowerAmount - newFloatingHashPowerAmount;
                oldAsset.regularHashPowerAmount = oldAsset.regularHashPowerAmount - newRegularHashPowerAmount;
                oldAsset.uptoStdHashPowerAmount = oldAsset.uptoStdHashPowerAmount - newUptoStdHashPowerAmount;
                //创建新资产
                StorageMap contractStateMap = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
                BigInteger totalSupply = contractStateMap.Get(TotalSupplyMapKey).AsBigInteger();
                Asset newAsset = new Asset();
                newAsset.assetId = totalSupply + 1;
                newAsset.owner = oldAsset.owner;
                newAsset.issuerKey = oldAsset.issuerKey; //发行地址
                newAsset.pledger = oldAsset.pledger; //质押地址,此质押地址应为空
                newAsset.ownershipStartDate = oldAsset.ownershipStartDate; //所有权开始日期
                newAsset.basicHashPowerAmount = newBalance; //基础算力
                newAsset.basicHashPowerExpiryDate = oldAsset.basicHashPowerExpiryDate; //基础算力有效日期
                newAsset.floatingHashPowerAmount = newFloatingHashPowerAmount; //活期上浮
                newAsset.floatingHashPowerExpiryDate = oldAsset.floatingHashPowerExpiryDate;
                newAsset.regularHashPowerAmount = newRegularHashPowerAmount; //定期上浮
                newAsset.regularHashPowerExpiryDate = oldAsset.regularHashPowerExpiryDate;
                newAsset.uptoStdHashPowerAmount = newUptoStdHashPowerAmount; //达标上浮
                newAsset.uptoStdHashPowerExpiryDate = oldAsset.uptoStdHashPowerExpiryDate;
                newAsset.unLockDate = oldAsset.unLockDate; //定期解锁时间
                newAsset.incomePercent = oldAsset.incomePercent; //分币比例
                newAsset.isIncomePledged = oldAsset.isIncomePledged; //是否质押分币权,如果是，则分币到质押地址
                newAsset.assetType = oldAsset.assetType; //算力类型，1，单挖；2,双挖; 3，高波
                newAsset.assetState = oldAsset.assetState; //Normal, //1：正常状态，可转让、可质押

                contractStateMap.Put(TotalSupplyMapKey, newAsset.assetId); //回写资产id

                //更新存储区资产信息
                assetMap.Put(newAsset.assetId.AsByteArray(), Helper.Serialize(newAsset));
                assetMap.Put(oldAsset.assetId.AsByteArray(), Helper.Serialize(oldAsset));

                //质押资产不可拆分，所以不需要增加质押地址资产索引
                AddOwnerNFTList(newAsset.owner, newAsset.assetId);
                //增加发行地址key NFT索引
                AddIssuerKeyNFTList(newAsset.issuerKey, newAsset.assetId);

                onMint(oldAsset.owner, 1);
                onNFTMint(oldAsset.owner, newAsset.assetId, newAsset);
                return newAsset.assetId; //新的资产id
            }

            return 0;
        }


        //合并资产，由owner或owner授权人发起
        //用户只可对正常状态的、除算力数量外其他属性均一致的算力资产进行合并。
        //合并操作将增加其中一个资产的算力值，并注销另一个资产
        //返回合并后资产的assetID
        /// <summary>
        /// 合并资产
        /// </summary>
        /// <param name="assetId1">资产id1</param>
        /// <param name="assetId2">资产id2</param>
        /// <returns></returns>
        public static BigInteger MergeAsset(byte[] addr, BigInteger assetId1, BigInteger assetId2,byte[] callingScript)
        {
            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);

            var assetData1 = assetMap.Get(assetId1.AsByteArray());
            var assetData2 = assetMap.Get(assetId2.AsByteArray());
            if (assetData1.Length > 0 && assetData2.Length > 0)
            {
                Asset asset1 = Helper.Deserialize(assetData1) as Asset;
                Asset asset2 = Helper.Deserialize(assetData2) as Asset;
                if (!IsTransfer(asset1) || !IsTransfer(asset2)) return 0;//判断是否过期，是否锁定
                if (IsPledger(asset1) || IsPledger(asset2)) return 0;
                if (asset1.owner != asset2.owner) return 0;
                if (!JudgeJurisdiction(asset1.owner, addr, assetId1, callingScript)) return 0;
                if (!JudgeJurisdiction(asset2.owner, addr, assetId2, callingScript)) return 0;

                //判断其他属性是否一致
                if (asset1.issuerKey != asset2.issuerKey) return 0;
                if (asset1.pledger != asset2.pledger) return 0;
                if (asset1.ownershipStartDate != asset2.ownershipStartDate) return 0;
                if (asset1.basicHashPowerExpiryDate != asset2.basicHashPowerExpiryDate) return 0;
                if (asset1.floatingHashPowerExpiryDate != asset2.floatingHashPowerExpiryDate) return 0;
                if (asset1.regularHashPowerExpiryDate != asset2.regularHashPowerExpiryDate) return 0;
                if (asset1.uptoStdHashPowerExpiryDate != asset2.uptoStdHashPowerExpiryDate) return 0;
                if (asset1.unLockDate != asset2.unLockDate) return 0;
                if (asset1.incomePercent != asset2.incomePercent) return 0;
                if (asset1.isIncomePledged != asset2.isIncomePledged) return 0;
                if (asset1.assetType != asset2.assetType) return 0;
                if (asset1.assetState != asset2.assetState) return 0;

                //将asset2基础算力合并到asset1
                asset1.basicHashPowerAmount = asset1.basicHashPowerAmount + asset2.basicHashPowerAmount;//合并基础算力
                asset1.floatingHashPowerAmount = asset1.floatingHashPowerAmount + asset2.floatingHashPowerAmount;
                asset1.regularHashPowerAmount = asset1.regularHashPowerAmount + asset2.regularHashPowerAmount;
                asset1.uptoStdHashPowerAmount = asset1.uptoStdHashPowerAmount + asset2.uptoStdHashPowerAmount;
                //注销asset2
                asset2.assetState = 0; //销毁

                //更新存储区资产信息
                assetMap.Put(assetId1.AsByteArray(), Helper.Serialize(asset1));
                assetMap.Put(assetId2.AsByteArray(), Helper.Serialize(asset2));

                //删除owner地址asset2资产索引
                RemoveOwnerNFTList(asset2.owner, asset2.assetId);
                //删除asset2资产所有的授权
                RemoveApproveForAsset(asset2.assetId);
                //删除发行asset2资产的地址NFT索引
                RemoveIssuerKeyNFTList(asset2.issuerKey, asset2.assetId);

                return assetId1;
            }
            return 0;
        }
        #endregion

        #region 销毁资产

        /// <summary>
        /// 销毁资产
        /// </summary>
        /// <param name="assetId">资产id</param>
        /// <returns></returns>
        public static bool DestoreyAsset(BigInteger assetId)
        {
            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = assetMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                Asset asset = Helper.Deserialize(data) as Asset;
                byte[] issuer = GetIssuerByKey(asset.issuerKey);
                if (asset.owner != issuer) return false;
                if (!IsTransfer(asset)) return false;//判断是否过期，是否在锁定期
                if (IsPledger(asset)) return false;
                if (!Runtime.CheckWitness(asset.owner)) return false;
                asset.assetState = 0;
                assetMap.Put(asset.assetId.AsByteArray(), Helper.Serialize(asset));

                //删除owner地址asset2资产索引
                RemoveOwnerNFTList(asset.owner, asset.assetId);
                //删除asset2资产所有的授权
                RemoveApproveForAsset(asset.assetId);
                //删除发行asset2资产的地址NFT索引
                RemoveIssuerKeyNFTList(asset.issuerKey, asset.assetId);
                return true;
            }
            return false;
        }

        #endregion

        #region 发行地址及发行地址key-address对操作

        /// <summary>
        /// 增加发行地址及发行地址key-address对
        /// </summary>
        /// <param name="issuerKey"></param>
        /// <param name="issuer"></param>
        /// <returns></returns>
        public static bool AddIssuerKeyAndIssuer(string issuerKey, byte[] issuer)
        {
            if (issuerKey.Length < 1) return false;
            if (!ValidateAddress(issuer)) return false;
            if (!Runtime.CheckWitness(superAdmin)) return false;
            AddIssuer(issuer);
            AddIssuerKey(issuerKey, issuer);
            return true;
        }

        /// <summary>
        /// 删除发行地址及发行地址key-address对
        /// </summary>
        /// <param name="issuerKey"></param>
        /// <param name="issuer"></param>
        /// <returns></returns>
        public static bool RemoveIssuerKeyAndIssuer(string issuerKey, byte[] issuer)
        {
            if (issuerKey.Length < 1) return false;
            if (!ValidateAddress(issuer)) return false;
            if (!Runtime.CheckWitness(superAdmin)) return false;
            RemoveIssuer(issuer);
            RemoveIssuerKey(issuerKey);
            return true;
        }

        #endregion

        ///获取某个账户的总的基础算力    
        public static BigInteger BalanceOf(byte[] addr)
        {
            if (!ValidateAddress(addr)) return 0;
            Iterator<byte[], byte[]> allOwnerAssets = GetOwnerNFTListByAddr(addr);
            BigInteger ownerBalance = 0;
            while (allOwnerAssets.Next())
            {
                BigInteger assetId = allOwnerAssets.Value.ToBigInteger();
                Asset asset = GetAssetByAssetId(assetId);
                if (IsEffectiveHashPower(asset))
                {
                    ownerBalance += asset.basicHashPowerAmount;
                }
            }
            return ownerBalance;
        }
    }
}
