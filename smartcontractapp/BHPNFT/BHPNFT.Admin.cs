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
        //铸币
        public static bool MintToken(byte[] issuerAddr, byte[] owner, BigInteger ownershipStartDate,
            BigInteger basicHashPowerAmount, BigInteger basicHashPowerExpiryDate,
            BigInteger floatingHashPowerAmount, BigInteger floatingHashPowerExpiryDate,
            BigInteger regularHashPowerAmount, BigInteger regularHashPowerExpiryDate,
             BigInteger uptoStdHashPowerAmount, BigInteger uptoStdHashPowerExpiryDate,
            BigInteger unLockDate, BigInteger incomePercent,
            BigInteger assetType, BigInteger assetState)
        {
            if (Runtime.CheckWitness(superAdmin)) issuerAddr = superAdmin; //超级管理员铸币那么发行者即为超级管理员
            if (Runtime.CheckWitness(superAdmin) || (Runtime.CheckWitness(issuerAddr) && IsMintAddress(issuerAddr)))
            {
                StorageMap sysStateMap = Storage.CurrentContext.CreateMap(StoragePrefixSysState);
                StorageMap tokenMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
                BigInteger totalSupply = sysStateMap.Get("totalSupply").AsBigInteger();
                Asset newToken = new Asset();
                newToken.asset_id = totalSupply + 1;
                newToken.owner = owner;
                newToken.spender = new byte[0];
                newToken.issuer = issuerAddr; //发行地址
                newToken.pledger = new byte[0]; //质押人地址
                newToken.ownershipStartDate = ownershipStartDate; //所有权开始日期
                newToken.basicHashPowerAmount = basicHashPowerAmount; //基础算力
                newToken.basicHashPowerExpiryDate = basicHashPowerExpiryDate; //基础算力有效日期
                newToken.floatingHashPowerAmount = floatingHashPowerAmount; //活期上浮
                newToken.floatingHashPowerExpiryDate = floatingHashPowerExpiryDate;
                newToken.regularHashPowerAmount = regularHashPowerAmount; //定期上浮
                newToken.regularHashPowerExpiryDate = regularHashPowerExpiryDate;
                newToken.uptoStdHashPowerAmount = uptoStdHashPowerAmount; //达标上浮
                newToken.uptoStdHashPowerExpiryDate = uptoStdHashPowerExpiryDate;
                newToken.unLockDate = unLockDate; //定期解锁时间
                newToken.incomePercent = incomePercent; //分币比例
                newToken.isIncomePledged = false; //是否质押分币权,如果是，则分币到质押地址
                newToken.assetType = assetType; //算力类型，1，单挖；2,双挖; 3，高波
                newToken.assetState = assetState; //Normal, //1：正常状态，可转让、可质押

                sysStateMap.Put("totalSupply", newToken.asset_id);
                tokenMap.Put(newToken.asset_id.AsByteArray(), Helper.Serialize(newToken));
                AddAddrNFTlist(owner, newToken.asset_id);

                onMint(owner, 1);
                onNFTMint(owner, newToken.asset_id, newToken);

                return true;
            }
            return false;

        }

        #endregion

        #region 授权发行地址增、减、查询, 判断地址是否为授权发行地址（对外）

        //查询授权发行地址
        public static Map<byte[], BigInteger> GetApproveMintAddr()
        {
            StorageMap addrApproveMintAddrs = Storage.CurrentContext.CreateMap(StoragePrefixMintAddr);
            var data = addrApproveMintAddrs.Get("approveMintKey");

            if (data.Length > 0)
            {
                return Helper.Deserialize(data) as Map<byte[], BigInteger>;
            }
            else
            {
                return new Map<byte[], BigInteger>();
            }
        }

        //增加授权发行地址(仅超级管理员可操作)
        public static bool ApproveMintAddrAdd(byte[] addr)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;
            StorageMap addrApproveMintAddrs = Storage.CurrentContext.CreateMap(StoragePrefixMintAddr);//0,存储addr拥有NFT总数//第一个位置存储个数

            Map<byte[], BigInteger> addrApproveMintlist = GetApproveMintAddr();
            byte[] number = new byte[1] { 0 };
            if (addrApproveMintlist.HasKey(number))
            {
                addrApproveMintlist[number] = addrApproveMintlist[number] + 1; //第一个位置存储个数
            }
            else
            {
                addrApproveMintlist[number] = 1;
            }
            addrApproveMintlist[addr] = 1;

            addrApproveMintAddrs.Put("approveMintKey", Helper.Serialize(addrApproveMintlist));
            return true;
        }

        //删除授权发行地址(仅超级管理员可操作)
        public static bool ApproveMintAddrRemove(byte[] addr)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap addrApproveMintNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixMintAddr);

            Map<byte[], BigInteger> addrApproveMintNFTlist = GetApproveMintAddr();
            byte[] number = new byte[1] { 0 };
            if (addrApproveMintNFTlist.HasKey(number))
            {
                addrApproveMintNFTlist[number] = addrApproveMintNFTlist[number] - 1;
                addrApproveMintNFTlist.Remove(addr);
                addrApproveMintNFTlistMap.Put("approveMintKey", Helper.Serialize(addrApproveMintNFTlist));
            }
            return true;
        }

        //判断发行地址是否在授权发行地址
        public static bool IsMintAddress(byte[] addr)
        {
            Map<byte[], BigInteger> addrApproveMintNFTlist = GetApproveMintAddr();
            if (addrApproveMintNFTlist.HasKey(addr))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 修改NFT属性 (仅超级管理员可修改) （对外）

        //修改NFT属性 
        public static bool ModifyNFTattribute(BigInteger tokenID, string attributeName, object attributeValue)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap tokenMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);

            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Asset token = Helper.Deserialize(data) as Asset;
                switch (attributeName)
                {
                    case "ownershipStartDate": token.ownershipStartDate = (BigInteger)attributeValue; break;
                    case "basicHashPowerAmount": token.basicHashPowerAmount = (BigInteger)attributeValue; break;
                    case "basicHashPowerExpiryDate": token.basicHashPowerExpiryDate = (BigInteger)attributeValue; break;
                    case "floatingHashPowerAmount": token.floatingHashPowerAmount = (BigInteger)attributeValue; break;
                    case "floatingHashPowerExpiryDate": token.floatingHashPowerExpiryDate = (BigInteger)attributeValue; break;
                    case "regularHashPowerAmount": token.regularHashPowerAmount = (BigInteger)attributeValue; break;
                    default: break;
                }
                switch (attributeName)
                {
                    case "regularHashPowerExpiryDate": token.regularHashPowerExpiryDate = (BigInteger)attributeValue; break;
                    case "uptoStdHashPowerAmount": token.uptoStdHashPowerAmount = (BigInteger)attributeValue; break;
                    case "uptoStdHashPowerExpiryDate": token.uptoStdHashPowerExpiryDate = (BigInteger)attributeValue; break;
                    case "unLockDate": token.unLockDate = (BigInteger)attributeValue; break;
                    case "incomePercent": token.incomePercent = (BigInteger)attributeValue; break;
                    case "isIncomePledged": token.isIncomePledged = (bool)attributeValue; break;
                    default: break;
                }
                switch (attributeName)
                {
                    case "assetType": token.assetType = (BigInteger)attributeValue; break;
                    case "assetState": token.assetState = (BigInteger)attributeValue; break;
                    default: break;
                }
                tokenMap.Put(token.asset_id.AsByteArray(), Helper.Serialize(token));
                onNFTModify(tokenID, attributeName, attributeValue);
                return true;
            }
            return false;
        }

        #endregion

        #region 修改合约属性（仅超级管理员） （对外）

        //修改名称
        public static bool SetName(string newName)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap sysStateMap = Storage.CurrentContext.CreateMap(StoragePrefixSysState);
            sysStateMap.Put("name", newName);

            onNameModify(newName);
            return true;
        }

        //修改Symbol
        public static bool SetSymbol(string newSymbol)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap sysStateMap = Storage.CurrentContext.CreateMap(StoragePrefixSysState);
            sysStateMap.Put("symbol", newSymbol);

            onSymbolModify(newSymbol);
            return true;
        }

        //修改支持标准
        public static bool SetSupportedStandards(string[] newSupportedStandards)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap sysStateMap = Storage.CurrentContext.CreateMap(StoragePrefixSysState);
            sysStateMap.Put("supportedStandards", Helper.Serialize(newSupportedStandards));

            onSupportedStandardsModify(newSupportedStandards);
            return true;
        }

        #endregion

        #region 合约升级，仅超级管理员操作。 （对外）

        //合约升级
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
