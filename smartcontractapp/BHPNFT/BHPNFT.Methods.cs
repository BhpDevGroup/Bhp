using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System.ComponentModel;
using System.Numerics;
using Helper = Bhp.SmartContract.Framework.Helper;

namespace BHPNFT
{
    public partial class BHPNFT : SmartContract
    {
        //代币操作方法

        //无入参只读类
        //- name() : 返回代币合约名称
        //- decimals() : 返回代币合约精度
        //- supportedStandards() : 返回代币合约支持的协议，如{"NEP-10"}
        //- symbol() : 返回代币合约的单位
        //- totalSupply() : 返回代币的总数
        //- isOpen(): 返回合约是否开放任意地址

        //单token_id只读类
        //- allowance(token_id) : 返回代币的授权信息
        //- ownerOf(token_id) : 返回代币的所有者
        //- properties(token_id) : 返回代币的只读属性
        //- uri(token_id): 返回代币的URI信息
        //- rwProperties(token_id) : 返回代币的可重写属性
        //- token(token_id) : 返回代币的所有信息的字典

        //所有权类
        //- balanceOf(owner) : 返回地址拥有NFT的个数
        //- tokenIDsOfOwner(owner) :获取指定地址所有NFT tokenID
        //- transfer(to, token_id, extra_arg): 转移一个NFT（为测试superAdmin可任意执行此方法）
        //- approve(token_receiver, token_id, revoke) : 授权第三方操作NFT所有权（为测试superAdmin可任意执行此方法）
        //- transferFrom(spender, from, to, token_id): 在授权后执行NFT所有权转移（为测试superAdmin可任意执行此方法）


        //代币合约所有者操作(为测试开放所有地址和superAdmin，当isOpen=false时仅superAdmin)
        // TOKEN_CONTRACT_OWNER operations:
        //    - mintToken(owner, properties, URI, extra_arg): 铸造新的NFT
        //    - modifyURI(token_id, token_data) : 修改URI信息
        //    - setRWProperties(token_id,token_data):修改可变属性
        //    - setProperties(token_id,token_data):设置不可变属性，仅为管理需要，仅superAdmin

        //设置操作（仅superAdmin）
        //setters:
        //- setName(name) : 设置代币合约的名字
        //- setSymbol(symbol) : 设置代币合约的单位
        //- setSupportedStandards(supported_standards) : 设置和合约自持的标准,一般是一个数组，总是首先包含“NEP-10”
        //- setIsOpen(bool) : 仅测试使用，为了在合约被滥用的情况下，可以关闭任意地址铸币、修改URL、rwProperties

        //不允许使用类变量
        //static string nameV = "NEL NFT Test";
        //static string symbolV = "NNT";
        //static string supportedStandardsV = "[\"NEP-10\"]";
        //static bool isOpenV = true;//是否开放铸币

        //事件
        //OnApprove = RegisterAction('approve', 'addr_from', 'addr_to', 'amount')
        //OnNFTApprove = RegisterAction('NFTapprove', 'addr_from', 'addr_to', 'tokenid')
        //OnTransfer = RegisterAction('transfer', 'addr_from', 'addr_to', 'amount')
        //OnNFTTransfer = RegisterAction('NFTtransfer', 'addr_from', 'addr_to', 'tokenid')
        //OnMint = RegisterAction('mint', 'addr_to', 'amount')
        //OnNFTMint = RegisterAction('NFTmint', 'addr_to', 'tokenid')

        #region 初始事件声明
        //铸造事件
        public delegate void deleMint(byte[] addrOwner, BigInteger amount);
        [DisplayName("mint")]
        public static event deleMint onMint;
        public delegate void deleNFTMint(byte[] addrOwner, BigInteger tokenID, Asset token);
        [DisplayName("NFTmint")]
        public static event deleNFTMint onNFTMint;

        //NFT修改事件
        public delegate void deleNFTModify(BigInteger tokenID, string elementName, object elementData);
        [DisplayName("NFTModify")]
        public static event deleNFTModify onNFTModify;

        //授权事件
        public delegate void deleApprove(byte[] addrOwner, byte[] addrApproved, BigInteger amount);
        [DisplayName("approve")]
        public static event deleApprove onApprove;
        public delegate void deleNFTApprove(byte[] addrOwner, byte[] addrApproved, BigInteger tokenID);
        [DisplayName("NFTapprove")]
        public static event deleNFTApprove onNFTApprove;

        //授权事件
        public delegate void delePledged(byte[] addrOwner, byte[] addrPledged, BigInteger amount);
        [DisplayName("pledged")]
        public static event delePledged onPledged;
        public delegate void deleNFTPledged(byte[] addrOwner, byte[] addrPledged, BigInteger tokenID);
        [DisplayName("NFTpledgede")]
        public static event deleNFTPledged onNFTPledged;

        //转账事件
        public delegate void deleTransfer(byte[] addrFrom, byte[] addrTo, BigInteger amount);
        [DisplayName("transfer")]
        public static event deleTransfer onTransfer;
        public delegate void deleNFTTransfer(byte[] addrFrom, byte[] addrTo, BigInteger tokenID);
        [DisplayName("NFTtransfer")]
        public static event deleNFTTransfer onNFTTransfer;

        //设置变更事件
        public delegate void deleNameModify(string newName);
        [DisplayName("nameModify")]
        public static event deleNameModify onNameModify;
        public delegate void deleSymbolModify(string newSymbol);
        [DisplayName("symbolModify")]
        public static event deleSymbolModify onSymbolModify;
        public delegate void deleSupportedStandardsModify(string[] newSupportedStandards);
        [DisplayName("supportedStandardsModify")]
        public static event deleSupportedStandardsModify onSupportedStandardsModify;
        //public delegate void deleIsOpenChange(bool isOpen);
        //[DisplayName("isOpenChange")]
        //public static event deleIsOpenChange onIsOpenChange;

        #endregion        

        #region  asset 定义
        public class Asset
        {
            public Asset()
            {
                assetId = 0;
                owner = new byte[0]; //资产拥有
                issuer = new byte[0]; //发行地址
                pledger = new byte[0]; //质押人地址
                ownershipStartDate = 20200101; //所有权开始日期
                basicHashPowerAmount = 0; //基础算力
                basicHashPowerExpiryDate = 0; //基础算力有效日期
                floatingHashPowerAmount = 0; //活期上浮
                floatingHashPowerExpiryDate = 0;
                regularHashPowerAmount = 0; //定期上浮
                regularHashPowerExpiryDate = 0;
                uptoStdHashPowerAmount = 0; //达标上浮
                uptoStdHashPowerExpiryDate = 0;
                unLockDate = 0; //定期解锁时间
                incomePercent = 0; //分币比例
                isIncomePledged = false; //是否质押分币权,如果是，则分币到质押地址
                assetType = 1; //算力类型，1，单挖；2,双挖; 3，高波
                assetState = 1; //Normal, //1：正常状态，可转让、可质押
                                //2：Locked, //定期锁定中，不可转让，可质押
                                //3：Pledged,//质押中，只可被质押地址拆分、转让或解质押        
                                //0：Destroyed //已销毁
            }
            //不能使用get set

            public BigInteger assetId;// 代币ID
            public byte[] owner; //代币所有权地址
            public byte[] issuer; //发行人地址
            public byte[] pledger; //质押人地址
            public BigInteger ownershipStartDate; //所有权开始日期
            public BigInteger basicHashPowerAmount; //基础算力
            public BigInteger basicHashPowerExpiryDate; //基础算力有效日期
            public BigInteger floatingHashPowerAmount; //活期上浮
            public BigInteger floatingHashPowerExpiryDate;
            public BigInteger regularHashPowerAmount; //定期上浮
            public BigInteger regularHashPowerExpiryDate;
            public BigInteger uptoStdHashPowerAmount; //达标上浮
            public BigInteger uptoStdHashPowerExpiryDate;
            public BigInteger unLockDate; //定期解锁时间
            public BigInteger incomePercent; //分币比例
            public bool isIncomePledged; //是否质押分币权,如果是，则分币到质押地址
            public BigInteger assetType; //算力类型，1，单挖；2,双挖; 3，高波
            public BigInteger assetState; //1.正常，2.定期锁定中,3.质押中 0.已销毁
        }

        #endregion

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
            return 2;
        }

        public static string[] SupportedStandards()
        {
            return new string[] { "BAS101", "BAS201" };
        }

        public static BigInteger TotalSupply()
        {
            StorageMap contractState = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
            return contractState.Get(TotalSupplyMapKey).AsBigInteger();
        }

        #endregion

        #region 查询 (对外)

        /// <summary>
        /// 通过资产id查询资产信息
        /// </summary>
        /// <param name="assetId">资产Id</param>
        /// <returns></returns>
        public static Asset GetAsset(BigInteger assetId)
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
        /// 返回代币的授权信息，代币授权地址
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public static byte[] Allowance(BigInteger assetId)
        {
            
             return new byte[0];
           
        }

        /// <summary>
        /// 获取代币所有权地址
        /// </summary>
        /// <param name="assetId>资产id</param>
        /// <returns></returns>
        public static byte[] OwnerOf(BigInteger assetId)
        {
            return GetAsset(assetId).owner;
        }


        ///// <summary>
        /////查询owner地址的nft资产列表 
        ///// </summary>
        ///// <param name="addr">owner地址</param>
        ///// <returns></returns>
        //public static Map<BigInteger, BigInteger> TokenIDsOfOwner(byte[] addr)
        //{
        //    return GetAssetsbyOwnerAddr(addr);
        //}

        ///// <summary>
        /////查询授权地址的nft资产列表 
        ///// </summary>
        ///// <param name="addr">授权地址</param>
        ///// <returns></returns>
        //public static Map<BigInteger, BigInteger> TokenIDsOfApproved(byte[] addr)
        //{
        //    return GetAssetsbyApprovedAddr(addr);
        //}

        ///// <summary>
        ///// 查询质押地址的nft资产列表
        ///// </summary>
        ///// <param name="addr">授权地址</param>
        ///// <returns></returns>
        //public static Map<BigInteger, BigInteger> TokenIDsOfPledged(byte[] addr)
        //{
        //    return GetAssetsbyPledger(addr);
        //}

        #endregion

        #region 基础查询（不对外）


        ///// <summary>
        ///// 查询地址所有的nft资产索引
        ///// </summary>
        ///// <param name="addr">owner地址</param>
        ///// <returns></returns>
        //public static Map<BigInteger, BigInteger> GetAssetsbyOwnerAddr(byte[] addr)
        //{
        //    StorageMap ownerAddrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixOwnerAddrList);
        //    var data = ownerAddrNFTlistMap.Get(addr);

        //    if (data.Length > 0)
        //    {
        //        return Helper.Deserialize(data) as Map<BigInteger, BigInteger>;
        //    }
        //    else
        //    {
        //        return new Map<BigInteger, BigInteger>();
        //    }

        //}

        ///// <summary>
        ///// 查询appove地址所有的nft资产索引（地址发行了那些资产）
        ///// </summary>
        ///// <param name="addr">发行地址</param>
        ///// <returns></returns>
        //public static Map<BigInteger, BigInteger> GetAssetsbyApprovedAddr(byte[] addr)
        //{
        //    StorageMap approvedAddrNFTMap = Storage.CurrentContext.CreateMap(StoragePrefixApprovedAddrNFTList);
        //    var data = approvedAddrNFTMap.Get(addr);

        //    if (data.Length > 0)
        //    {
        //        return Helper.Deserialize(data) as Map<BigInteger, BigInteger>;
        //    }
        //    else
        //    {
        //        return new Map<BigInteger, BigInteger>();
        //    }

        //}

        ///// <summary>
        /////查询pledged地址所有的nft资产索引 
        ///// </summary>
        ///// <param name="addr">质押地址</param>
        ///// <returns></returns>
        //public static Map<BigInteger, BigInteger> GetAssetsbyPledger(byte[] addr)
        //{
        //    StorageMap pledgedAddrNFTMap = Storage.CurrentContext.CreateMap(StoragePrefixPledgedAddrNFTList);
        //    var data = pledgedAddrNFTMap.Get(addr);

        //    if (data.Length > 0)
        //    {
        //        return Helper.Deserialize(data) as Map<BigInteger, BigInteger>;
        //    }
        //    else
        //    {
        //        return new Map<BigInteger, BigInteger>();
        //    }
        //}

        #endregion

        #region 所有权地址、质押地址、授权地址、发行地址 资产索引操作 (不对外)

        /// <summary>
        /// 增加ower地址nft资产索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        private static bool AddOwnerAddrNFTlist(byte[] addr, BigInteger assetId)
        {
            StorageMap ownerAddrNFTMap = Storage.CurrentContext.CreateMap(StoragePrefixOwnerAddrList);
            ownerAddrNFTMap.Put(addr.Concat(assetId.AsByteArray()), assetId);
            return true;
        }

        /// <summary>
        /// 删除owner地址拥有的nft资产索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        private static bool RemoveOwnerAddrNFTlist(byte[] addr, BigInteger assetId)
        {
            StorageMap ownerAddrNFTMap = Storage.CurrentContext.CreateMap(StoragePrefixOwnerAddrList);//0,存储addr拥有NFT总数
            ownerAddrNFTMap.Delete(addr.Concat(assetId.AsByteArray()));
            return true;
        }

        /// <summary>
        /// 增加approved地址nft资产索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        //private static bool AddApprovedAddrNFTlist(byte[] addr, BigInteger assetId)
        //{
        //    StorageMap approvedAddrNFTMap = Storage.CurrentContext.CreateMap(StoragePrefixApprovedAddrNFTList);//0,存储addr拥有NFT总数//第一个位置存储个数
        //    approvedAddrNFTMap.Put(addr.Concat(assetId.AsByteArray()), assetId);
        //    return true;
        //}

        ///// <summary>
        ///// 删除approved地址拥有的nft资产索引
        ///// </summary>
        ///// <param name="addr">地址</param>
        ///// <param name="assetId">资产id</param>
        //private static bool RemoveApprovedAddrNFTlist(byte[] addr, BigInteger assetId)
        //{
        //    StorageMap approvedAddrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixApprovedAddrNFTList);//0,存储addr拥有NFT总数
        //    approvedAddrNFTlistMap.Delete(addr.Concat(assetId.AsByteArray()));
        //    return true;
        //}

        /// <summary>
        /// 增加pledged地址nft资索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        private static bool AddPledgedAddrNFTlist(byte[] addr, BigInteger assetId)
        {
            StorageMap pledgedAddrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixPledgedAddrNFTList);//0,存储addr拥有NFT总数//第一个位置存储个数
            pledgedAddrNFTlistMap.Put(addr.Concat(assetId.AsByteArray()), assetId);
            return true;
        }

        /// <summary>
        /// 删除pledged地址拥有的nft资产索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        private static bool RemovePledgedAddrNFTlist(byte[] addr, BigInteger assetId)
        {
            StorageMap pledgedAddrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixPledgedAddrNFTList);//0,存储addr拥有NFT总数
            pledgedAddrNFTlistMap.Delete(addr.Concat(assetId.AsByteArray()));
            return true;
        }

        /// <summary>
        /// 增加issuer地址nft资索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        private static bool AddIssuerAddrNFTlist(byte[] addr, BigInteger assetId)
        {
            StorageMap issuerAddrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixIssuerAddrNFTList);//0,存储addr拥有NFT总数//第一个位置存储个数
            issuerAddrNFTlistMap.Put(addr.Concat(assetId.AsByteArray()), assetId);
            return true;
        }

        /// <summary>
        /// 删除issuer地址拥有的nft资产索引
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="assetId">资产id</param>
        private static bool RemoveIssuerAddrNFTlist(byte[] addr, BigInteger assetId)
        {
            StorageMap issuerAddrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixIssuerAddrNFTList);//0,存储addr拥有NFT总数
            issuerAddrNFTlistMap.Delete(addr.Concat(assetId.AsByteArray()));
            return true;
        }

        #endregion


        #region 基础判断（不对外）

        /// <summary>
        /// 判断资产是否能转账，判断过期，及是否在锁定期，过期及锁定期内不能转账
        /// </summary>
        /// <param name="assetId">资产id</param>
        /// <returns></returns>
        private static bool IsTransfer(BigInteger assetId) 
        {
            return true;
        }

        /// <summary>
        /// 判断是否授权了资产
        /// </summary>
        /// <param name="owner">所有权地址</param>
        /// <param name="issuer">授权地址</param>
        /// <param name="assetId">资产id</param>
        /// <returns></returns>
        private static bool IsApprove(byte[] owner, byte[] approve, BigInteger assetId)
        {
            //判断是否授权了所有资产
            StorageMap approveAllMap = Storage.CurrentContext.CreateMap(StoragePrefixApproveAll);
            if (approveAllMap.Get(owner.Concat(approve)) != null)
            {
                return true;
            }
            //判断部分授权中是否授权了该资产
            StorageMap approveAssetMap = Storage.CurrentContext.CreateMap(StoragePrefixApproveAsset);
            if (approveAssetMap.Get(owner.Concat(approve).Concat(assetId.AsByteArray())) != null)
            {
                return true;
            }
            return false;
        }


        #endregion


        #region 转账 （对外）

        //转账给他人，只能由owner调用，只能转让正常状态下的资产，支持拆分转让
        /// <summary>
        /// 所有权地址转账(全部转出)
        /// </summary>
        /// <param name="to">接收地址</param>
        /// <param name="assetId">资产id</param>
        /// <returns></returns>
        public static bool Transfer(byte[] to, BigInteger assetId, BigInteger amount)
        {
            //判断地址是否合法
            if (!ValidateAddress(to)) return false;

            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = assetMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                //旧的资产
                Asset oldAsset = Helper.Deserialize(data) as Asset;
                if (!(Runtime.CheckWitness(oldAsset.owner))) return false;
                if (oldAsset.assetState != 1) return false;
                if (!IsTransfer(assetId)) return false; //判断是否能转账，过期及锁定期不能转账
                if (oldAsset.basicHashPowerAmount < amount || amount < 0) return false;
                if (oldAsset.basicHashPowerAmount == amount)
                {
                    //直接修改资产的owner地址
                    var addrFrom = oldAsset.owner;
                    oldAsset.owner = to;
                    assetMap.Put(assetId.AsByteArray(), Helper.Serialize(oldAsset));
                    //删除from 地址资产索引，增加to地址资产索引
                    RemoveOwnerAddrNFTlist(addrFrom, assetId);
                    AddOwnerAddrNFTlist(to, assetId);

                    //如果该资产已经授权，删除部分授权中该资产授权？？？？？（不删也可以，owner已经改变，新的owner在授权中不会有授权的记录）
                    
                    
                    //此处不正确，不能查询到是删除谁的授权资产
                   // RemoveApprovedAddrNFTlist(addrFrom, assetId);

                    onTransfer(addrFrom, to, 1);
                    onNFTTransfer(addrFrom, to, assetId);

                    return true;

                }
                //部分转让
                else
                {
                    //修改老资产
                    oldAsset.basicHashPowerAmount = oldAsset.basicHashPowerAmount - amount;

                    //创建新资产
                    StorageMap contractStateMap = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
                    BigInteger totalSupply = contractStateMap.Get(TotalSupplyMapKey).AsBigInteger();
                    Asset newAsset = new Asset();
                    newAsset.assetId = totalSupply + 1;
                    newAsset.owner = to; //资产所有者为to地址
                    newAsset.issuer = oldAsset.issuer; //发行地址
                    newAsset.pledger = new byte[0]; //质押人地址
                    newAsset.ownershipStartDate = oldAsset.ownershipStartDate; //所有权开始日期
                    newAsset.basicHashPowerAmount = amount; //基础算力
                    newAsset.basicHashPowerExpiryDate = oldAsset.basicHashPowerExpiryDate; //基础算力有效日期
                    newAsset.floatingHashPowerAmount = oldAsset.floatingHashPowerAmount; //活期上浮
                    newAsset.floatingHashPowerExpiryDate = oldAsset.floatingHashPowerExpiryDate;
                    newAsset.regularHashPowerAmount = oldAsset.regularHashPowerAmount; //定期上浮
                    newAsset.regularHashPowerExpiryDate = oldAsset.regularHashPowerExpiryDate;
                    newAsset.uptoStdHashPowerAmount = oldAsset.uptoStdHashPowerAmount; //达标上浮
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

                    //质押资产转账，所以不需要增加质押地址资产索引
                    AddOwnerAddrNFTlist(to, newAsset.assetId);  //增加to地址资产索引
                    //授权地址，则增加授权地址资产索引（转账后新资产清空了授权地址）
                    //if (newAsset.spender != new byte[0])
                    //{
                    //    AddApprovedAddrNFTlist(newAsset.spender, newAsset.assetId);
                    //}

                    onTransfer(oldAsset.owner, to, 1);
                    onNFTTransfer(oldAsset.owner, to, assetId);
                    return true; //新的资产id
                }
            }

            return false;
        }

        //授权地址转账，质押地址转账,
        public static bool TransferFrom(byte[] to, BigInteger assetId)
        {
            if (to.Length != 20) return false;

            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = assetMap.Get(assetId.AsByteArray());
            //token 存在
            if (data.Length > 0)
            {
                Asset asset = Helper.Deserialize(data) as Asset;
                byte[] approvedAddr = asset.spender;
                BigInteger assetState = asset.assetState;
                //自己只能转出资产状态为1，正常状态的资产
                if (assetState == 1)
                {
                    if (!Runtime.CheckWitness(asset.spender)) return false;
                    var addrFrom = asset.owner;
                    asset.owner = to;
                    asset.spender = new byte[0];//删除授权地址

                    assetMap.Put(assetId.AsByteArray(), Helper.Serialize(asset));
                    RemoveOwnerAddrNFTlist(addrFrom, assetId);
                    AddOwnerAddrNFTlist(to, assetId);

                    RemoveApprovedAddrNFTlist(approvedAddr, assetId); //删除授权地址该资产索引

                    onTransfer(addrFrom, to, 1);
                    onNFTTransfer(addrFrom, to, assetId);
                    return true;
                }
                if (assetState == 3)
                {
                    if (!Runtime.CheckWitness(asset.pledger)) return false;
                    var addrFrom = asset.owner;
                    asset.owner = to;
                    asset.pledger = new byte[0]; //删除质押地址

                    assetMap.Put(assetId.AsByteArray(), Helper.Serialize(asset));
                    RemoveOwnerAddrNFTlist(addrFrom, assetId);
                    AddOwnerAddrNFTlist(to, assetId);
                    RemovePledgedAddrNFTlist(asset.pledger, assetId); //转出后，质押地址不在对资产有操作权，删除质押地址资产索引
                    onTransfer(addrFrom, to, 1);
                    onNFTTransfer(addrFrom, to, assetId);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region  授权 （对外）

        //授权，revoke = true，取消授权，revoke = fasle,授权
        public static bool Approve(byte[] approve, BigInteger assetId, bool revoke)
        {
            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var assetData = assetMap.Get(assetId.AsByteArray());
            if (assetData.Length > 0) {
                Asset asset = Helper.Deserialize(assetData) as Asset;
                //判断是都为所有权地址在操作
                if (!Runtime.CheckWitness(asset.owner)) return false;
                byte[] owner = asset.owner;
                StorageMap approveAssetMap = Storage.CurrentContext.CreateMap(StoragePrefixApproveAsset);
                if (revoke) //取消授权
                {
                    approveAssetMap.Delete(owner.Concat(approve).Concat(assetId.AsByteArray()));
                }
                else
                {
                    approveAssetMap.Put(owner.Concat(approve).Concat(assetId.AsByteArray()), assetId);
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
        private static bool RemoveApproveForAsset(byte[] owner, byte[] approve, BigInteger assetId)
        {
            StorageMap approveAssetMap = Storage.CurrentContext.CreateMap(StoragePrefixApproveAsset);
            approveAssetMap.Delete(owner.Concat(approve).Concat(assetId.AsByteArray()));
            return true;
        }


        //授权某地址操作账户里所有资产
        //授权的操作包括转让、分拆、合并、质押
        public static bool SetApprovalForAll(byte[] owner, byte[] approve)
        {
            if (!Runtime.CheckWitness(owner)) return false;
            StorageMap approveAllMap = Storage.CurrentContext.CreateMap(StoragePrefixApproveAll);//0,存储addr拥有NFT总数//第一个位置存储个数
            approveAllMap.Put(owner.Concat(approve), approve);
            return true;
        }

        //删除全部授权
        public static bool RemoveApprovalForAll(byte[] owner, byte[] approve)
        {
            if (!Runtime.CheckWitness(owner)) return false;
            StorageMap approveAllMap = Storage.CurrentContext.CreateMap(StoragePrefixApproveAll);//0,存储addr拥有NFT总数//第一个位置存储个数
            approveAllMap.Delete(owner.Concat(approve));
            return true;
        }

        public static Map<byte [], BigInteger> GetApproveAllAddrs(byte[] owner)
        {
            StorageMap approveAllMap = Storage.CurrentContext.CreateMap(StoragePrefixApproveAll);
            var data = approveAllMap.Get(owner);
            if (data.Length > 0)
            {
                return Helper.Deserialize(data) as Map<byte[] , BigInteger>;
            }
            else
            {
                return new Map<byte[], BigInteger>();
            }
        }

   
        public static Map<BigInteger, BigInteger> GetApproveAsset(string ownerAndApprove)
        {
            StorageMap approveAssetMap = Storage.CurrentContext.CreateMap(StoragePrefixApproveAsset);
            var data = approveAssetMap.Get(ownerAndApprove);
            if (data.Length > 0)
            {
                return Helper.Deserialize(data) as Map<BigInteger, BigInteger>;
            }
            else
            {
                return new Map<BigInteger, BigInteger>();
            }
        }



        #endregion

        #region 质押、解质押 （对外）

        // 质押资产
        public static bool Pledge(byte[] addr, BigInteger tokenID, bool isIncomePledged)
        {
            if (addr.Length != 20) return false;

            StorageMap tokenMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Asset token = Helper.Deserialize(data) as Asset;
                //
                if (!(Runtime.CheckWitness(token.owner) || Runtime.CheckWitness(token.spender))) return false;

                token.pledger = addr;
                token.assetState = 3; //质押状态
                token.isIncomePledged = isIncomePledged;

                //此处应增加质押地址所拥有资产索引
                AddPledgedAddrNFTlist(addr, tokenID);

                tokenMap.Put(tokenID.AsByteArray(), Helper.Serialize(token));

                onPledged(token.owner, token.pledger, 1);
                onNFTPledged(token.owner, token.pledger, tokenID);
                return true;
            }
            return false;
        }

        //解除质押
        public static bool UnPledge(BigInteger tokenID)
        {

            StorageMap tokenMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Asset token = Helper.Deserialize(data) as Asset;

                byte[] pledgerAddr = token.pledger;
                if (!Runtime.CheckWitness(token.pledger)) return false;

                token.pledger = new byte[0]; //质押地址置空
                token.isIncomePledged = false;
                token.assetState = 1; //质押状态改为正常
                //删除质押地址的资产索引
                RemovePledgedAddrNFTlist(pledgerAddr, tokenID);
                tokenMap.Put(tokenID.AsByteArray(), Helper.Serialize(token));

                onPledged(token.owner, token.pledger, 1);
                onNFTPledged(token.owner, token.pledger, tokenID);

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
        public static BigInteger SplitAsset(BigInteger assetId, BigInteger remainBalance, BigInteger newBalance)
        {
            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);

            var data = assetMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                //旧的资产
                Asset oldAsset = Helper.Deserialize(data) as Asset;
                //返回-1表示资产分拆失败
                if (!(Runtime.CheckWitness(oldAsset.owner) || Runtime.CheckWitness(oldAsset.spender))) return -1;
                if (oldAsset.assetType != 1) return -1;
                if (remainBalance < 0 || newBalance < 0) return -1;
                if ((remainBalance + newBalance) != oldAsset.basicHashPowerAmount) return -1; //分拆数量不正确

                //修改老资产
                oldAsset.basicHashPowerAmount = remainBalance;

                //创建新资产
                StorageMap contractStateMap = Storage.CurrentContext.CreateMap(StoragePrefixContractState);
                BigInteger totalSupply = contractStateMap.Get(TotalSupplyMapKey).AsBigInteger();
                Asset newAsset = new Asset();
                newAsset.assetId = totalSupply + 1;
                newAsset.owner = oldAsset.owner;
                newAsset.spender = oldAsset.spender;
                newAsset.issuer = oldAsset.issuer; //发行地址
                newAsset.pledger = oldAsset.pledger; //质押人地址
                newAsset.ownershipStartDate = oldAsset.ownershipStartDate; //所有权开始日期
                newAsset.basicHashPowerAmount = newBalance; //基础算力
                newAsset.basicHashPowerExpiryDate = oldAsset.basicHashPowerExpiryDate; //基础算力有效日期
                newAsset.floatingHashPowerAmount = oldAsset.floatingHashPowerAmount; //活期上浮
                newAsset.floatingHashPowerExpiryDate = oldAsset.floatingHashPowerExpiryDate;
                newAsset.regularHashPowerAmount = oldAsset.regularHashPowerAmount; //定期上浮
                newAsset.regularHashPowerExpiryDate = oldAsset.regularHashPowerExpiryDate;
                newAsset.uptoStdHashPowerAmount = oldAsset.uptoStdHashPowerAmount; //达标上浮
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
                AddOwnerAddrNFTlist(oldAsset.owner, newAsset.assetId);
                //如果有授权地址，则增加授权地址资产索引
                if (newAsset.spender != new byte[0])
                {
                    AddApprovedAddrNFTlist(newAsset.spender, newAsset.assetId);
                }
                onMint(oldAsset.owner, 1);
                onNFTMint(oldAsset.owner, newAsset.assetId, newAsset);
                return newAsset.assetId; //新的资产id
            }

            return -1;
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
        public static BigInteger MergeAsset(BigInteger assetId1, BigInteger assetId2)
        {
            StorageMap assetMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);

            var assetData1 = assetMap.Get(assetId1.AsByteArray());
            var assetData2 = assetMap.Get(assetId2.AsByteArray());
            if (assetData1.Length > 0 && assetData2.Length > 0)
            {

                Asset asset1 = Helper.Deserialize(assetData1) as Asset;
                Asset asset2 = Helper.Deserialize(assetData2) as Asset;
                if (asset1.owner != asset2.owner) return -1;
                if (asset1.spender != asset2.spender) return -1;
                if (!(Runtime.CheckWitness(asset1.owner) || Runtime.CheckWitness(asset1.spender))) return -1;

                //判断其他属性是否一致
                if (asset1.issuer != asset2.issuer) return -1;
                if (asset1.pledger != asset2.pledger) return -1;
                if (asset1.ownershipStartDate != asset2.ownershipStartDate) return -1;
                if (asset1.basicHashPowerExpiryDate != asset2.basicHashPowerExpiryDate) return -1;
                if (asset1.floatingHashPowerAmount != asset2.floatingHashPowerAmount) return -1;
                if (asset1.floatingHashPowerExpiryDate != asset2.floatingHashPowerExpiryDate) return -1;
                if (asset1.regularHashPowerAmount != asset2.regularHashPowerAmount) return -1;
                if (asset1.regularHashPowerExpiryDate != asset2.regularHashPowerExpiryDate) return -1;
                if (asset1.uptoStdHashPowerAmount != asset2.uptoStdHashPowerAmount) return -1;
                if (asset1.uptoStdHashPowerExpiryDate != asset2.uptoStdHashPowerExpiryDate) return -1;
                if (asset1.unLockDate != asset2.unLockDate) return -1;
                if (asset1.incomePercent != asset2.incomePercent) return -1;
                if (asset1.isIncomePledged != asset2.isIncomePledged) return -1;
                if (asset1.assetType != asset2.assetType) return -1;
                if (asset1.assetState != asset2.assetState) return -1;

                //将asset2基础算力合并到asset1
                asset1.basicHashPowerAmount = asset1.basicHashPowerAmount + asset2.basicHashPowerAmount;//合并基础算力

                //注销asset2
                asset2.assetState = 0; //销毁

                //更新存储区资产信息
                assetMap.Put(assetId1.AsByteArray(), Helper.Serialize(asset1));
                assetMap.Put(assetId2.AsByteArray(), Helper.Serialize(asset2));

                //删除owner地址asset2资产索引
                RemoveOwnerAddrNFTlist(asset2.owner, assetId2);
                if (asset2.spender != new byte[0])
                {
                    RemoveApprovedAddrNFTlist(asset2.spender, assetId2);
                }
                return assetId1;
            }
            return -1;
        }
        #endregion
    }
}
