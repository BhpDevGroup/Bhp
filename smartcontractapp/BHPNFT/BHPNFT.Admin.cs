using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System.Numerics;
using Helper = Bhp.SmartContract.Framework.Helper;
using Bhp.SmartContract.Framework.Services.System;

namespace BHPNFT
{
    public partial class BHPNFT : SmartContract
    {
        #region 铸币（对外）

        /// <summary>
        /// 铸币
        /// </summary>
        /// <param name="issuerAddr"></param>
        /// <param name="ownerAddr"></param>
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
        public static bool IssueAsset(byte[] issuerAddr, byte[] ownerAddr, BigInteger ownershipStartDate,
            BigInteger basicHashPowerAmount, BigInteger basicHashPowerExpiryDate,
            BigInteger floatingHashPowerAmount, BigInteger floatingHashPowerExpiryDate,
            BigInteger regularHashPowerAmount, BigInteger regularHashPowerExpiryDate,
             BigInteger uptoStdHashPowerAmount, BigInteger uptoStdHashPowerExpiryDate,
            BigInteger unLockDate, BigInteger incomePercent,
            BigInteger assetType, BigInteger assetState)
        {
            if (Runtime.CheckWitness(issuerAddr) && IsMintAddress(issuerAddr))
            {
                StorageMap contractStateMap = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
                StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
                BigInteger totalSupply = contractStateMap.Get(TotalSupplyMapKey).AsBigInteger();
                Asset newAsset = new Asset();
                newAsset.assetId = totalSupply + 1;
                newAsset.owner = ownerAddr;
                newAsset.issuer = issuerAddr; //发行地址
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
                assetMap.Put(newAsset.assetId.AsByteArray(), Helper.Serialize(newAsset));
                AddOwnerAddrNFTlist(ownerAddr, newAsset.assetId);

                onMint(ownerAddr, 1);
                onNFTMint(ownerAddr, newAsset.assetId, newAsset);

                return true;
            }
            return false;

        }

        #endregion

        #region 授权发行地址增、减、查询, 判断地址是否为授权发行地址（对外）


        ///// <summary>
        ///// 查询授权发行地址
        ///// </summary>
        ///// <returns></returns>
        //public static Map<byte[], BigInteger> GetIssuers()
        //{
        //    StorageMap approveMintAddrsMap = Storage.CurrentContext.CreateMap(StoragePrefixMintAddrs);
        //    var data = approveMintAddrsMap.Get(ApproveMintKeyMapKey);

        //    if (data.Length > 0)
        //    {
        //        return Helper.Deserialize(data) as Map<byte[], BigInteger>;
        //    }
        //    else
        //    {
        //        return new Map<byte[], BigInteger>();
        //    }
        //}

     
        /// <summary>
        /// 增加授权发行地址(仅超级管理员可操作)
        /// </summary>
        /// <param name="addr">授权发行地址</param>
        /// <returns></returns>
        public static bool AddIssuer(byte[] addr)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;
            StorageMap approveMintAddrsMap = Storage.CurrentContext.CreateMap(StoragePrefixMintAddrs);
            approveMintAddrsMap.Put(addr, addr);
            return true;
        }

        /// <summary>
        /// 删除授权发行地址(仅超级管理员可操作)
        /// </summary>
        /// <param name="addr">授权发行地址</param>
        /// <returns></returns>
        public static bool RemoveIssuer(byte[] addr)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;
            StorageMap approveMintAddrsMap = Storage.CurrentContext.CreateMap(StoragePrefixMintAddrs);
            approveMintAddrsMap.Delete(addr);
            return true;
        }

        /// <summary>
        /// 判断发行地址是否在授权发行地址
        /// </summary>
        /// <param name="addr">地址</param>
        /// <returns></returns>
        public static bool IsMintAddress(byte[] addr)
        {
            StorageMap approveMintAddrsMap = Storage.CurrentContext.CreateMap(StoragePrefixMintAddrs);
            if (approveMintAddrsMap.Get(addr) == null)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 修改NFT属性 (仅超级管理员可修改) （对外）

        /// <summary>
        /// 修改NFT属性
        /// </summary>
        /// <param name="assetId">资产Id</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attributeValue">变更后的属性值</param>
        /// <returns></returns>
        public static bool ModifyNFTAttribute(BigInteger assetId, string attributeName, object attributeValue)
        {
            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = assetMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                Asset asset = Helper.Deserialize(data) as Asset;
                if (!Runtime.CheckWitness(asset.issuer)) return false;
                switch (attributeName)
                {
                    case "ownershipStartDate": asset.ownershipStartDate = (BigInteger)attributeValue; break;
                    case "basicHashPowerAmount": asset.basicHashPowerAmount = (BigInteger)attributeValue; break;
                    case "basicHashPowerExpiryDate": asset.basicHashPowerExpiryDate = (BigInteger)attributeValue; break;
                    case "floatingHashPowerAmount": asset.floatingHashPowerAmount = (BigInteger)attributeValue; break;
                    case "floatingHashPowerExpiryDate": asset.floatingHashPowerExpiryDate = (BigInteger)attributeValue; break;
                    case "regularHashPowerAmount": asset.regularHashPowerAmount = (BigInteger)attributeValue; break;
                    default: break;
                }
                switch (attributeName)
                {
                    case "regularHashPowerExpiryDate": asset.regularHashPowerExpiryDate = (BigInteger)attributeValue; break;
                    case "uptoStdHashPowerAmount": asset.uptoStdHashPowerAmount = (BigInteger)attributeValue; break;
                    case "uptoStdHashPowerExpiryDate": asset.uptoStdHashPowerExpiryDate = (BigInteger)attributeValue; break;
                    case "unLockDate": asset.unLockDate = (BigInteger)attributeValue; break;
                    case "incomePercent": asset.incomePercent = (BigInteger)attributeValue; break;
                    case "isIncomePledged": asset.isIncomePledged = (bool)attributeValue; break;
                    default: break;
                }
                switch (attributeName)
                {
                    case "assetType": asset.assetType = (BigInteger)attributeValue; break;
                    case "assetState": asset.assetState = (BigInteger)attributeValue; break;
                    default: break;
                }
                assetMap.Put(asset.assetId.AsByteArray(), Helper.Serialize(asset));
                onNFTModify(assetId, attributeName, attributeValue);
                return true;
            }
            return false;
        }

        #endregion

        #region 修改合约属性（仅超级管理员） （对外）

        /// <summary>
        /// 修改合约Name
        /// </summary>
        /// <param name="newName">新的名称</param>
        /// <returns></returns>
        public static bool SetName(string newName)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap contractStateMap = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
            contractStateMap.Put("name", newName);

            onNameModify(newName);
            return true;
        }

        /// <summary>
        /// 修改Symbol
        /// </summary>
        /// <param name="newSymbol">新的Symbol</param>
        /// <returns></returns>
        public static bool SetSymbol(string newSymbol)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap contractStateMap = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
            contractStateMap.Put("symbol", newSymbol);

            onSymbolModify(newSymbol);
            return true;
        }

        /// <summary>
        /// 修改支持标准
        /// </summary>
        /// <param name="newSupportedStandards">新的支持标准</param>
        /// <returns></returns>
        public static bool SetSupportedStandards(string[] newSupportedStandards)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap contractStateMap = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
            contractStateMap.Put("supportedStandards", Helper.Serialize(newSupportedStandards));

            onSupportedStandardsModify(newSupportedStandards);
            return true;
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
    }
}
