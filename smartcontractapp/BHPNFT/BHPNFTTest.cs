using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System.Numerics;
using System.ComponentModel;
using Helper = Bhp.SmartContract.Framework.Helper;
using Bhp.SmartContract.Framework.Services.System;

namespace BHPNFT
{
    public class BHPNFTTest : SmartContract
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
        public delegate void deleNFTMint(byte[] addrOwner, BigInteger tokenID, Token token);
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

        //超级管理員
        static readonly byte[] superAdmin = Helper.ToScriptHash("AWWx2F1Ph9oJtbU8H2mcJGDgDeFDH8geWs");

        #region  token 定义
        public class Token
        {
            public Token()
            {
                token_id = 0;
                owner = new byte[0]; //资产拥有
                approved = new byte[0];//授权地址
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

            public BigInteger token_id;// 代币ID
            public byte[] owner; //代币所有权地址
            public byte[] approved;//代币授权处置权地址
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
            StorageMap sysState = Storage.CurrentContext.CreateMap("sysState");
            var data = sysState.Get("name");
            if (data.Length > 0) return data.AsString();
            return "myBNN";
        }

        public static string symbol()
        {
            StorageMap sysStateMap = Storage.CurrentContext.CreateMap("sysState");
            var data = sysStateMap.Get("symbol");
            if (data.Length > 0) return data.AsString();
            return "myBNS";
        }

        public static BigInteger decimals()
        {
            return 0;
        }

        public static string[] supportedStandards()
        {
            StorageMap sysStateMap = Storage.CurrentContext.CreateMap("sysState");
            var data = sysStateMap.Get("supportedStandards");
            if (data.Length > 0) return Helper.Deserialize(data) as string[];
            return new string[] { "BRC-20" };
        }

        public static BigInteger totalSupply()
        {
            StorageMap sysStateMap = Storage.CurrentContext.CreateMap("sysState");
            return sysStateMap.Get("totalSupply").AsBigInteger();
        }
        
        #endregion

        #region 查询 (对外)

        //通过资产id查询资产信息
        public static Token getToken(BigInteger tokenID)
        {
            StorageMap tokenMap = Storage.CurrentContext.CreateMap("token");

            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Token token = Helper.Deserialize(data) as Token;

                return token;
            }
            return new Token();
        }

        //返回代币的授权信息，代币授权地址
        public static byte[] allowance(BigInteger tokenID)
        {
            Token token = getToken(tokenID);
            if (token.approved.Length > 0)
            {
                return token.owner.Concat(token.approved);
            }
            else
            {
                return new byte[0];
            }
        }

        //获取代币所有权地址
        public static byte[] ownerOf(BigInteger tokenID)
        {
            return getToken(tokenID).owner;
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
            StorageMap addrNFTlistMap = Storage.CurrentContext.CreateMap("addrNFTlist");
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
            StorageMap approvedAddrNFTlist = Storage.CurrentContext.CreateMap("approvedAddrNFTlist");
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
            StorageMap pledgedAddrNFTlist = Storage.CurrentContext.CreateMap("pledgedAddrNFTlist");
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
            StorageMap addrNFTlistMap = Storage.CurrentContext.CreateMap("addrNFTlist");//0,存储addr拥有NFT总数//第一个位置存储个数

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
            StorageMap addrNFTlistMap = Storage.CurrentContext.CreateMap("addrNFTlist");//0,存储addr拥有NFT总数

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
            StorageMap approvedAddrNFTlistMap = Storage.CurrentContext.CreateMap("approvedAddrNFTlist");//0,存储addr拥有NFT总数//第一个位置存储个数

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
            StorageMap approvedAddrNFTlistMap = Storage.CurrentContext.CreateMap("approvedAddrNFTlist");//0,存储addr拥有NFT总数

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
            StorageMap pledgedAddrNFTlistMap = Storage.CurrentContext.CreateMap("pledgedAddrNFTlist");//0,存储addr拥有NFT总数//第一个位置存储个数

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
            StorageMap pledgedAddrNFTlistMap = Storage.CurrentContext.CreateMap("pledgedAddrNFTlist");//0,存储addr拥有NFT总数

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

            StorageMap tokenMap = Storage.CurrentContext.CreateMap("token");
            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Token token = Helper.Deserialize(data) as Token;
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

            StorageMap tokenMap = Storage.CurrentContext.CreateMap("token");
            var data = tokenMap.Get(tokenID.AsByteArray());
            //token 存在
            if (data.Length > 0)
            {
                Token token = Helper.Deserialize(data) as Token;
                if (!Runtime.CheckWitness(token.approved)) return false;
                byte[] approvedAddr = token.approved;
                BigInteger assetState = token.assetState;
                //自己只能转出资产状态为1，正常状态的资产
                if (assetState == 1)
                {
                    var addrFrom = token.owner;
                    token.owner = addrTo;
                    token.approved = new byte[0];//删除授权地址

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

            StorageMap tokenMap = Storage.CurrentContext.CreateMap("token");
            var data = tokenMap.Get(tokenID.AsByteArray());
            //token 存在
            if (data.Length > 0)
            {
                Token token = Helper.Deserialize(data) as Token;
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

            StorageMap tokenMap = Storage.CurrentContext.CreateMap("token");
            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Token token = Helper.Deserialize(data) as Token;
                if (!Runtime.CheckWitness(token.owner)) return false;

                if (!revoke)
                {
                    token.approved = addr;
                    approvedAddrNFTlistAdd(addr, tokenID); //增加授权地址索引
                }
                else
                {
                    token.approved = new byte[0];
                    approvedAddrNFTlistRemove(addr, tokenID);
                }
                tokenMap.Put(tokenID.AsByteArray(), Helper.Serialize(token));
                onApprove(token.owner, token.approved, 1);
                onNFTApprove(token.owner, token.approved, tokenID);

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

            StorageMap tokenMap = Storage.CurrentContext.CreateMap("token");
            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Token token = Helper.Deserialize(data) as Token;
                //
                if (!(Runtime.CheckWitness(token.owner) || Runtime.CheckWitness(token.approved))) return false;

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
        public static bool unPledge(BigInteger tokenID) {

            StorageMap tokenMap = Storage.CurrentContext.CreateMap("token");
            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Token token = Helper.Deserialize(data) as Token;

                byte [] pledgerAddr = token.pledger;
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

        #region 铸币（对外）
        //铸币
        public static bool mintToken(byte[] issuerAddr, byte[] owner, BigInteger ownershipStartDate, 
            BigInteger basicHashPowerAmount, BigInteger basicHashPowerExpiryDate, 
            BigInteger floatingHashPowerAmount,BigInteger floatingHashPowerExpiryDate,
            BigInteger regularHashPowerAmount, BigInteger regularHashPowerExpiryDate,
             BigInteger uptoStdHashPowerAmount, BigInteger uptoStdHashPowerExpiryDate,
            BigInteger unLockDate, BigInteger incomePercent,
            BigInteger assetType, BigInteger assetState)
        {
            if (Runtime.CheckWitness(superAdmin)) issuerAddr = superAdmin; //超级管理员铸币那么发行者即为超级管理员
            if (Runtime.CheckWitness(superAdmin)  || (Runtime.CheckWitness(issuerAddr) && isMintAddress(issuerAddr)))
            {
                StorageMap sysStateMap = Storage.CurrentContext.CreateMap("sysState");
                StorageMap tokenMap = Storage.CurrentContext.CreateMap("token");
                BigInteger totalSupply = sysStateMap.Get("totalSupply").AsBigInteger();
                Token newToken = new Token();
                newToken.token_id = totalSupply + 1;
                newToken.owner = owner;
                newToken.approved = new byte[0];
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

                sysStateMap.Put("totalSupply", newToken.token_id);
                tokenMap.Put(newToken.token_id.AsByteArray(), Helper.Serialize(newToken));
                addrNFTlistAdd(owner, newToken.token_id);

                onMint(owner, 1);
                onNFTMint(owner, newToken.token_id, newToken);

                return true;
            }
            return false;
           
        }

        #endregion

        #region 授权发行地址增、减、查询, 判断地址是否为授权发行地址（对外）

        //查询授权发行地址
        public static Map<byte[], BigInteger> getApproveMintAddr()
        {
            StorageMap addrApproveMintAddrs = Storage.CurrentContext.CreateMap("addrApproveMintAddrs");
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
            StorageMap addrApproveMintAddrs = Storage.CurrentContext.CreateMap("addrApproveMintAddrs");//0,存储addr拥有NFT总数//第一个位置存储个数

            Map<byte[], BigInteger> addrApproveMintlist = getApproveMintAddr();
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

            StorageMap addrApproveMintNFTlistMap = Storage.CurrentContext.CreateMap("addrApproveMintAddrs");

            Map<byte[], BigInteger> addrApproveMintNFTlist = getApproveMintAddr();
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
        public static bool isMintAddress(byte[] addr)
        {
            Map<byte[], BigInteger> addrApproveMintNFTlist = getApproveMintAddr();
            if (addrApproveMintNFTlist.HasKey(addr))
            {
                return true;
            }
            return false;
        }

        #endregion 

        #region 修改NFT属性 (仅超级管理员可修改) （对外）

        //修改NFT属性 
        public static bool modifyNFTattribute(BigInteger tokenID, string attributeName, object attributeValue)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap tokenMap = Storage.CurrentContext.CreateMap("token");

            var data = tokenMap.Get(tokenID.AsByteArray());
            if (data.Length > 0)
            {
                Token token = Helper.Deserialize(data) as Token;
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
                tokenMap.Put(token.token_id.AsByteArray(), Helper.Serialize(token));
                onNFTModify(tokenID, attributeName, attributeValue);
                return true;
            }
            return false;
        }
        
        #endregion

        #region 修改合约属性（仅超级管理员） （对外）
        
        //修改名称
        public static bool setName(string newName)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap sysStateMap = Storage.CurrentContext.CreateMap("sysState");
            sysStateMap.Put("name", newName);

            onNameModify(newName);
            return true;
        }

        //修改Symbol
        public static bool setSymbol(string newSymbol)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap sysStateMap = Storage.CurrentContext.CreateMap("sysState");
            sysStateMap.Put("symbol", newSymbol);

            onSymbolModify(newSymbol);
            return true;
        }

        //修改支持标准
        public static bool setSupportedStandards(string[] newSupportedStandards)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            StorageMap sysStateMap = Storage.CurrentContext.CreateMap("sysState");
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

        private static object migrateContract(object[] args)
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
                    return name();
                }
                if (operation == "symbol")
                {
                    return symbol();
                }
                if (operation == "decimals")
                {
                    return decimals();
                }
                if (operation == "supportedStandards")
                {
                    return supportedStandards();
                }
                if (operation == "totalSupply")
                {
                    return totalSupply();
                }
                #endregion

                #region 转账
                if (operation == "transfer")
                {
                    if (args.Length != 2) return false;
                    return transfer((byte[])args[0], (BigInteger)args[1]);
                }
                if (operation == "transferFrom")
                {
                    if (args.Length != 2) return false;
                    return transferFrom((byte[])args[0], (BigInteger)args[1]);
                }
                if (operation == "transferTo")
                {
                    if (args.Length != 2) return false;
                    return transferTo((byte[])args[0], (BigInteger)args[1]);
                }
                #endregion

                #region 授权
                if (operation == "approve")
                {
                    if (args.Length != 2 && args.Length != 3) return false;
                    if (args.Length == 2) return approve((byte[])args[0], (BigInteger)args[1], false);
                    return approve((byte[])args[0], (BigInteger)args[1], (bool)args[2]);
                }

                #endregion

                #region 质押、解质押
               
                if (operation == "pledger")
                {
                    if (args.Length != 3) return false;
                    return pledgerNFT((byte[])args[0],(BigInteger)args[1],(bool)args[2]);
                }
                if (operation == "unpledger")
                {
                    if (args.Length != 1) return false;
                    return unPledge((BigInteger)args[0]);
                }

                #endregion

                #region 查询
                //单token_id只读类
                if (operation == "allowance")
                {
                    if (args.Length != 1) return false;
                    return allowance((BigInteger)args[0]);
                }
                if (operation == "ownerOf")
                {
                    if (args.Length != 1) return false;
                    return ownerOf((BigInteger)args[0]);
                }

                if (operation == "token")
                {
                    if (args.Length != 1) return false;
                    return getToken((BigInteger)args[0]);
                }
                //所有权类
                if (operation == "balanceOf")
                {
                    if (args.Length != 1) return false;
                    return balanceOf((byte[])args[0]);
                }
                if (operation == "tokenIDsOfOwner")
                {
                    if (args.Length != 1) return false;
                    return tokenIDsOfOwner((byte[])args[0]);
                }
                if (operation == "tokenIDsOfApproved")
                {
                    if (args.Length != 1) return false;
                    return tokenIDsOfApproved((byte[])args[0]);
                }
                if (operation == "tokenIDsOfPledged")
                {
                    if (args.Length != 1) return false;
                    return tokenIDsOfPledged((byte[])args[0]);
                }
                #endregion

                #region 铸币
                //代币合约所有者操作(superAdmin)
                if (operation == "mintToken")
                {
                    if (args.Length != 15) return false;
                    return mintToken((byte[])args[0], (byte[])args[1], (BigInteger)args[2], (BigInteger)args[3],
                        (BigInteger)args[4], (BigInteger)args[5], (BigInteger)args[6],(BigInteger)args[7], (BigInteger)args[8], 
                        (BigInteger)args[9],(BigInteger)args[10], (BigInteger)args[11], (BigInteger)args[12], (BigInteger)args[13],(BigInteger)args[14]);
                }
                #endregion

                #region 修改NFT属性

                if (operation == "modifyNFTattribute")
                {
                    if (args.Length != 3) return false;
                    return modifyNFTattribute((BigInteger)args[0],(string)args[1], (object)args[2]);
                }
                #endregion

                #region 修改合约属性
                //设置操作（仅superAdmin）
                if (operation == "setName")
                {
                    if (args.Length != 1) return false;
                    return setName((string)args[0]);
                }
                if (operation == "setSymbol")
                {
                    if (args.Length != 1) return false;
                    return setSymbol((string)args[0]);
                }
                if (operation == "setSupportedStandards")
                {
                    if (args.Length != 1) return false;
                    return setSupportedStandards((string[])args[0]);
                }

                #endregion

                #region 合约升级

                if (operation == "migrate")
                {
                    return migrateContract(args);
                }

                #endregion

                #region 授权地址操作

                //获取发行地址
                if (operation == "getApproveMintAddr")
                {
                    return getApproveMintAddr();
                }
                //增加发行地址（仅超级管理员）
                if (operation == "approveMintAddrAdd")
                {
                    if (args.Length != 1) return false;
                    return ApproveMintAddrAdd((byte[])args[0]);
                }
                //删除发行地址（仅超级管理员）
                if (operation == "approveMintAddrRemove")
                {
                    if (args.Length != 1) return false;
                    return ApproveMintAddrRemove((byte[])args[0]);
                }
                //判断地址是否为发行地址
                if (operation == "isMintAddress")
                {
                    if (args.Length != 1) return false;
                    return isMintAddress((byte[])args[0]);
                }

                #endregion

            }
            return false;
        }

    }
}
