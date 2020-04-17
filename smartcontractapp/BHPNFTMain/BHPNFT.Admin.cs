using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using Bhp.SmartContract.Framework.Services.System;
using System.Numerics;
using Helper = Bhp.SmartContract.Framework.Helper;

namespace BhpHashPowerNFT
{
    public partial class HashPowerContract : SmartContract
    {
        
        #region 铸币（对外）

        /// <summary>
        /// 铸币
        /// </summary>
        /// <param name="issuerKey"></param>
        /// <param name="owner"></param>
        /// <param name="ownershipStartDate"></param>
        /// <param name="basicHashPowerAmount"></param>
        /// <param name="basicHashPowerExpiryDate"></param>
        /// <param name="floatingHashPowerAmount"></param>
        /// <param name="floatingHashPowerExpiryDate"></param>
        /// <param name="regularHashPowerAmount"></param>
        /// <param name="regularHashPowerExpiryDate"></param>
        /// <param name="uptoStdHashPowerAmount"></param>
        /// <param name="uptoStdHashPowerExpiryDate"></param>
        /// <param name="unLockDate"></param>
        /// <param name="incomePercent"></param>
        /// <param name="assetType"></param>
        /// <param name="assetState"></param>
        /// <returns></returns>
        public static bool IssueAsset(string issuerKey,byte[] owner, BigInteger ownershipStartDate,
            BigInteger basicHashPowerAmount, BigInteger basicHashPowerExpiryDate,
            BigInteger floatingHashPowerAmount, BigInteger floatingHashPowerExpiryDate,
            BigInteger regularHashPowerAmount, BigInteger regularHashPowerExpiryDate,
             BigInteger uptoStdHashPowerAmount, BigInteger uptoStdHashPowerExpiryDate,
            BigInteger unLockDate, BigInteger incomePercent,
            BigInteger assetType, BigInteger assetState)
        {
            //通过issuerKey查询byte[] issuer
            byte[] issuer = GetIssuerByKey(issuerKey);
            if (issuer == null) return false;
            if (!ValidateAddress(issuer)) return false;
            if (!ValidateAddress(owner)) return false;
            if (Runtime.CheckWitness(issuer) && IsMintAddr(issuer))
            {
                StorageMap contractStateMap = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
                StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
                BigInteger totalSupply = contractStateMap.Get(TotalSupplyMapKey).AsBigInteger();
                Asset newAsset = new Asset();
                newAsset.assetId = totalSupply + 1;
                newAsset.owner = owner;
                newAsset.issuerKey = issuerKey; //发行地址key
                newAsset.pledger = new byte[0]; //质押人地址
                newAsset.ownershipStartDate = ownershipStartDate; //所有权开始日期
                newAsset.basicHashPowerAmount = basicHashPowerAmount; //基础算力
                newAsset.basicHashPowerExpiryDate = basicHashPowerExpiryDate; //基础算力有效日期
                newAsset.floatingHashPowerAmount = floatingHashPowerAmount; //活期上浮
                newAsset.floatingHashPowerExpiryDate = floatingHashPowerExpiryDate;
                newAsset.regularHashPowerAmount = regularHashPowerAmount; //定期上浮
                newAsset.regularHashPowerExpiryDate = regularHashPowerExpiryDate;
                newAsset.uptoStdHashPowerAmount = uptoStdHashPowerAmount; //达标上浮
                newAsset.uptoStdHashPowerExpiryDate = uptoStdHashPowerExpiryDate;
                newAsset.unLockDate = unLockDate; //定期解锁时间
                newAsset.incomePercent = incomePercent; //分币比例
                newAsset.isIncomePledged = false; //是否质押分币权,如果是，则分币到质押地址
                newAsset.assetType = assetType; //算力类型，1，单挖；2,双挖; 3，高波
                newAsset.assetState = assetState; //Normal, //1：正常状态，可转让、可质押

                contractStateMap.Put(TotalSupplyMapKey, newAsset.assetId); //回写资产id
                assetMap.Put(newAsset.assetId.AsByteArray(), Helper.Serialize(newAsset));//资产写入存储区

                //增加owner地址资产索引
                AddOwnerNFTList(owner, newAsset.assetId);
                //增加issuerkey资产索引
                AddIssuerKeyNFTList(issuerKey, newAsset.assetId);

                //通知客户端铸币事件
                onMint(owner, 1);
                onNFTMint(owner, newAsset.assetId, newAsset);
                return true;
            }
            return false;

        }
        #endregion

        #region 合约升级，仅超级管理员操作。 （对外）

        /// <summary>
        /// 合约升级
        /// </summary>
        /// <param name="script"></param>
        /// <param name="plist"></param>
        /// <param name="rtype"></param>
        /// <param name="cps"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="author"></param>
        /// <param name="email"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private static bool Migrate(byte[] script, byte[] plist, byte rtype, ContractPropertyState cps, string name, string version, string author, string email, string description)
        {
            var contract = Contract.Migrate(script, plist, rtype, cps, name, version, author, email, description);
            return true;
        }

        private static object MigrateContract(object[] args)
        {
            if (!Runtime.CheckWitness(superAdmin))
                return false;

            if (args.Length < 9) return false;

            byte[] script = Blockchain.GetContract(ExecutionEngine.ExecutingScriptHash).Script;
            byte[] new_script = (byte[])args[0];
            if (script == new_script)
                return false;

            byte[] parameter_list = (byte[])args[1];
            byte return_type = (byte)args[2];
            ContractPropertyState cps = (ContractPropertyState)args[3];
            string name = (string)args[4];
            string version = (string)args[5];
            string author = (string)args[6];
            string email = (string)args[7];
            string description = (string)args[8];
            return Migrate(new_script, parameter_list, return_type, cps, name, version, author, email, description);
        }

        #endregion

        #region 修改NFT属性 （对外）

        /// <summary>
        /// 修改NFT属性
        /// </summary>
        /// <param name="assetId">资产Id</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attributeValue">变更后的属性值</param>
        /// <returns></returns>
        public static bool ModifyNFTAttribute(BigInteger assetId, Map<string, object> attribute)
        {
            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = assetMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                Asset asset = Helper.Deserialize(data) as Asset;
                byte[] issuer = GetIssuerByKey(asset.issuerKey);
                if (!Runtime.CheckWitness(issuer)) return false;
                if (attribute.HasKey("basicHashPowerExpiryDate")) 
                {
                    asset.basicHashPowerExpiryDate = (BigInteger)attribute["basicHashPowerExpiryDate"];
                }
                if (attribute.HasKey("floatingHashPowerExpiryDate"))
                {
                    asset.floatingHashPowerExpiryDate = (BigInteger)attribute["floatingHashPowerExpiryDate"];
                }
                if (attribute.HasKey("regularHashPowerExpiryDate"))
                {
                    asset.regularHashPowerExpiryDate = (BigInteger)attribute["regularHashPowerExpiryDate"];
                }
                if (attribute.HasKey("uptoStdHashPowerExpiryDate"))
                {
                    asset.uptoStdHashPowerExpiryDate = (BigInteger)attribute["uptoStdHashPowerExpiryDate"];
                }
                if (attribute.HasKey("unLockDate"))
                {
                    asset.unLockDate = (BigInteger)attribute["unLockDate"];
                }
                assetMap.Put(asset.assetId.AsByteArray(), Helper.Serialize(asset));

              //  onNFTModify(assetId, attributeName, attributeValue);
                return true;
            }
            return false;
        }

        #endregion

    }
}
