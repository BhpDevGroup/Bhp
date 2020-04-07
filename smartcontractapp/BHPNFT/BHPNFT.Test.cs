using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System.Numerics;
using System.ComponentModel;
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
                asset_id = 0;
                owner = new byte[0]; //资产拥有
                spender = new byte[0];//授权地址
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

            public BigInteger asset_id;// 代币ID
            public byte[] owner; //代币所有权地址
            public byte[] spender;//代币授权处置权地址
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
        public static string name()
        {
            StorageMap sysState = Storage.CurrentContext.CreateMap(StoragePrefixSysState);
            var data = sysState.Get("name");
            if (data.Length > 0) return data.AsString();
            return "BTCT";
        }

        public static string symbol()
        {
            StorageMap sysStateMap = Storage.CurrentContext.CreateMap(StoragePrefixSysState);
            var data = sysStateMap.Get("symbol");
            if (data.Length > 0) return data.AsString();
            return "BTCT";
        }

        public static BigInteger decimals()
        {
            return 2;
        }

        public static string[] supportedStandards()
        {
            StorageMap sysStateMap = Storage.CurrentContext.CreateMap(StoragePrefixSysState);
            var data = sysStateMap.Get("supportedStandards");
            if (data.Length > 0) return Helper.Deserialize(data) as string[];
            return new string[] { "BRC-20" };
        }

        public static BigInteger totalSupply()
        {
            StorageMap sysStateMap = Storage.CurrentContext.CreateMap(StoragePrefixSysState);
            return sysStateMap.Get("totalSupply").AsBigInteger();
        }

        #endregion

        #region 查询 (对外)

        //通过资产id查询资产信息
        public static Asset GetAsset(BigInteger assetId)
        {
            StorageMap tokenMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);

            var data = tokenMap.Get(assetId.AsByteArray());
            if (data.Length > 0)
            {
                Asset token = Helper.Deserialize(data) as Asset;

                return token;
            }
            return new Asset();
        }

        //返回代币的授权信息，代币授权地址
        public static byte[] allowance(BigInteger assetId)
        {
            Asset token = GetAsset(assetId);
            if (token.spender.Length > 0)
            {
                return token.owner.Concat(token.spender);
            }
            else
            {
                return new byte[0];
            }
        }

        //获取代币所有权地址
        public static byte[] ownerOf(BigInteger tokenID)
        {
            return GetAsset(tokenID).owner;
        }

        //查询owner地址拥有的nft个数
        public static BigInteger balanceOf(byte[] addr)
        {
            Map<BigInteger, BigInteger> addrNFTlist = getAddrNFTlist(addr);
            if (addrNFTlist.HasKey(0))
            {
                return addrNFTlist[0];
            }
            return 0;
        }

        //查询owner地址的nft资产列表
        public static Map<BigInteger, BigInteger> tokenIDsOfOwner(byte[] addr)
        {
            return getAddrNFTlist(addr);
        }

        //查询授权地址的nft资产列表
        public static Map<BigInteger, BigInteger> tokenIDsOfApproved(byte[] addr)
        {
            return getApprovedAddrNFTlist(addr);
        }

        //查询质押地址的nft资产列表
        public static Map<BigInteger, BigInteger> tokenIDsOfPledged(byte[] addr)
        {
            return getPledgedAddrNFTlist(addr);
        }

        #endregion

        #region 基础查询（不对外）

        //查询地址所有的nft资产索引
        public static Map<BigInteger, BigInteger> getAddrNFTlist(byte[] addr)
        {
            StorageMap addrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixAddrList);
            var data = addrNFTlistMap.Get(addr);

            if (data.Length > 0)
            {
                return Helper.Deserialize(data) as Map<BigInteger, BigInteger>;
            }
            else
            {
                return new Map<BigInteger, BigInteger>();
            }

        }

        //查询appove地址所有的nft资产索引（地址发行了那些资产）
        public static Map<BigInteger, BigInteger> getApprovedAddrNFTlist(byte[] addr)
        {
            StorageMap approvedAddrNFTlist = Storage.CurrentContext.CreateMap(StoragePrefixApprovedAddrList);
            var data = approvedAddrNFTlist.Get(addr);

            if (data.Length > 0)
            {
                return Helper.Deserialize(data) as Map<BigInteger, BigInteger>;
            }
            else
            {
                return new Map<BigInteger, BigInteger>();
            }

        }

        //查询pledged地址所有的nft资产索引
        public static Map<BigInteger, BigInteger> getPledgedAddrNFTlist(byte[] addr)
        {
            StorageMap pledgedAddrNFTlist = Storage.CurrentContext.CreateMap(StoragePrefixPledgedAddrList);
            var data = pledgedAddrNFTlist.Get(addr);

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

        #region 所有权地址、质押地址、授权地址 资产索引操作 (不对外)

        //增加owner地址nft资产索引
        public static void addrNFTlistAdd(byte[] addr, BigInteger tokenID)
        {
            StorageMap addrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixAddrList);//0,存储addr拥有NFT总数//第一个位置存储个数

            Map<BigInteger, BigInteger> addrNFTlist = getAddrNFTlist(addr);
            if (addrNFTlist.HasKey(0))
            {
                addrNFTlist[0] = addrNFTlist[0] + 1; //第一个位置存储个数
            }
            else
            {
                addrNFTlist[0] = 1;
            }
            addrNFTlist[tokenID] = 1;

            addrNFTlistMap.Put(addr, Helper.Serialize(addrNFTlist));
        }

        //删除owner地址拥有的nft资产索引
        public static void addrNFTlistRemove(byte[] addr, BigInteger tokenID)
        {
            StorageMap addrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixAddrList);//0,存储addr拥有NFT总数

            Map<BigInteger, BigInteger> addrNFTlist = getAddrNFTlist(addr);

            if (addrNFTlist.HasKey(tokenID))
            {
                addrNFTlist[0] = addrNFTlist[0] - 1;
                addrNFTlist.Remove(tokenID);
                addrNFTlistMap.Put(addr, Helper.Serialize(addrNFTlist));
            }
        }

        //增加approved地址nft资产索引
        public static void approvedAddrNFTlistAdd(byte[] addr, BigInteger tokenID)
        {
            StorageMap approvedAddrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixApprovedAddrList);//0,存储addr拥有NFT总数//第一个位置存储个数

            Map<BigInteger, BigInteger> approvedAddrNFTlist = getApprovedAddrNFTlist(addr);
            if (approvedAddrNFTlist.HasKey(0))
            {
                approvedAddrNFTlist[0] = approvedAddrNFTlist[0] + 1; //第一个位置存储个数
            }
            else
            {
                approvedAddrNFTlist[0] = 1;
            }
            approvedAddrNFTlist[tokenID] = 1;

            approvedAddrNFTlistMap.Put(addr, Helper.Serialize(approvedAddrNFTlist));
        }

        //删除approved地址拥有的nft资产索引
        public static void approvedAddrNFTlistRemove(byte[] addr, BigInteger tokenID)
        {
            StorageMap approvedAddrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixApprovedAddrList);//0,存储addr拥有NFT总数

            Map<BigInteger, BigInteger> approvedAddrNFTlist = getApprovedAddrNFTlist(addr);

            if (approvedAddrNFTlist.HasKey(tokenID))
            {
                approvedAddrNFTlist[0] = approvedAddrNFTlist[0] - 1;
                approvedAddrNFTlist.Remove(tokenID);
                approvedAddrNFTlistMap.Put(addr, Helper.Serialize(approvedAddrNFTlist));
            }
        }

        //增加pledged地址nft资索引
        public static void pledgedAddrNFTlistAdd(byte[] addr, BigInteger tokenID)
        {
            StorageMap pledgedAddrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixPledgedAddrList);//0,存储addr拥有NFT总数//第一个位置存储个数

            Map<BigInteger, BigInteger> pledgedAddrNFTlist = getPledgedAddrNFTlist(addr);
            if (pledgedAddrNFTlist.HasKey(0))
            {
                pledgedAddrNFTlist[0] = pledgedAddrNFTlist[0] + 1; //第一个位置存储个数
            }
            else
            {
                pledgedAddrNFTlist[0] = 1;
            }
            pledgedAddrNFTlist[tokenID] = 1;

            pledgedAddrNFTlistMap.Put(addr, Helper.Serialize(pledgedAddrNFTlist));
        }

        //删除pledged地址拥有的nft资产索引
        public static void pledgedAddrNFTlistRemove(byte[] addr, BigInteger tokenID)
        {
            StorageMap pledgedAddrNFTlistMap = Storage.CurrentContext.CreateMap(StoragePrefixPledgedAddrList);//0,存储addr拥有NFT总数

            Map<BigInteger, BigInteger> pledgedAddrNFTlist = getPledgedAddrNFTlist(addr);

            if (pledgedAddrNFTlist.HasKey(tokenID))
            {
                pledgedAddrNFTlist[0] = pledgedAddrNFTlist[0] - 1;
                pledgedAddrNFTlist.Remove(tokenID);
                pledgedAddrNFTlistMap.Put(addr, Helper.Serialize(pledgedAddrNFTlist));
            }
        }

        #endregion

        #region 转账 （对外）

        //所有权地址转账
        public static bool transfer(byte[] addrTo, BigInteger tokenID)
        {
            //判断地址长度
            if (addrTo.Length != 20) return false;

            StorageMap tokenMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Asset token = Helper.Deserialize(data) as Asset;
                if (!Runtime.CheckWitness(token.owner)) return false;
                BigInteger assetState = token.assetState;
                //自己只能转出资产状态为1，正常状态的资产
                if (assetState == 1)
                {
                    var addrFrom = token.owner;
                    token.owner = addrTo;
                    tokenMap.Put(tokenID.AsByteArray(), Helper.Serialize(token));
                    addrNFTlistRemove(addrFrom, tokenID);
                    addrNFTlistAdd(addrTo, tokenID);
                    onTransfer(addrFrom, addrTo, 1);
                    onNFTTransfer(addrFrom, addrTo, tokenID);
                    return true;
                }
            }
            return false;
        }

        //授权地址转账
        public static bool transferFrom(byte[] addrTo, BigInteger tokenID)
        {
            if (addrTo.Length != 20) return false;

            StorageMap tokenMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = tokenMap.Get(tokenID.AsByteArray());
            //token 存在
            if (data.Length > 0)
            {
                Asset token = Helper.Deserialize(data) as Asset;
                if (!Runtime.CheckWitness(token.spender)) return false;
                byte[] approvedAddr = token.spender;
                BigInteger assetState = token.assetState;
                //自己只能转出资产状态为1，正常状态的资产
                if (assetState == 1)
                {
                    var addrFrom = token.owner;
                    token.owner = addrTo;
                    token.spender = new byte[0];//删除授权地址

                    tokenMap.Put(tokenID.AsByteArray(), Helper.Serialize(token));
                    addrNFTlistRemove(addrFrom, tokenID);
                    addrNFTlistAdd(addrTo, tokenID);

                    approvedAddrNFTlistRemove(approvedAddr, tokenID); //删除授权地址该资产索引

                    onTransfer(addrFrom, addrTo, 1);
                    onNFTTransfer(addrFrom, addrTo, tokenID);
                    return true;
                }
            }
            return false;
        }

        //质押地址转账
        public static bool transferTo(byte[] addrTo, BigInteger tokenID)
        {
            if (addrTo.Length != 20) return false;

            StorageMap tokenMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = tokenMap.Get(tokenID.AsByteArray());
            //token 存在
            if (data.Length > 0)
            {
                Asset token = Helper.Deserialize(data) as Asset;
                if (!Runtime.CheckWitness(token.pledger)) return false;
                byte[] pledgerAddr = token.pledger;
                BigInteger assetState = token.assetState;
                //只能转出资产状态为3，质押状态的资产
                if (assetState == 3)
                {
                    var addrFrom = token.owner;
                    token.owner = addrTo;
                    token.pledger = new byte[0]; //删除质押地址

                    tokenMap.Put(tokenID.AsByteArray(), Helper.Serialize(token));
                    addrNFTlistRemove(addrFrom, tokenID);
                    addrNFTlistAdd(addrTo, tokenID);
                    pledgedAddrNFTlistRemove(pledgerAddr, tokenID); //转出后，质押地址不在对资产有操作权，删除质押地址资产索引
                    onTransfer(addrFrom, addrTo, 1);
                    onNFTTransfer(addrFrom, addrTo, tokenID);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region  授权 （对外）

        //授权，revoke = true，取消授权，revoke = fasle,授权
        public static bool approve(byte[] addr, BigInteger tokenID, bool revoke)
        {
            if (addr.Length != 20) return false;

            StorageMap tokenMap = Storage.CurrentContext.CreateMap(StoragePrefixAsset);
            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Asset token = Helper.Deserialize(data) as Asset;
                if (!Runtime.CheckWitness(token.owner)) return false;

                if (!revoke)
                {
                    token.spender = addr;
                    approvedAddrNFTlistAdd(addr, tokenID); //增加授权地址索引
                }
                else
                {
                    token.spender = new byte[0];
                    approvedAddrNFTlistRemove(addr, tokenID);
                }
                tokenMap.Put(tokenID.AsByteArray(), Helper.Serialize(token));
                onApprove(token.owner, token.spender, 1);
                onNFTApprove(token.owner, token.spender, tokenID);

                return true;
            }

            return false;
        }

        #endregion

        #region 质押、解质押 （对外）

        // 质押资产
        public static bool pledgerNFT(byte[] addr, BigInteger tokenID, bool isIncomePledged)
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
                pledgedAddrNFTlistAdd(addr, tokenID);

                tokenMap.Put(tokenID.AsByteArray(), Helper.Serialize(token));

                onPledged(token.owner, token.pledger, 1);
                onNFTPledged(token.owner, token.pledger, tokenID);
                return true;
            }
            return false;
        }

        //解除质押
        public static bool unPledge(BigInteger tokenID)
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
                pledgedAddrNFTlistRemove(pledgerAddr, tokenID);
                tokenMap.Put(tokenID.AsByteArray(), Helper.Serialize(token));

                onPledged(token.owner, token.pledger, 1);
                onNFTPledged(token.owner, token.pledger, tokenID);

                return true;
            }
            return false;
        }

        #endregion
    }
}
