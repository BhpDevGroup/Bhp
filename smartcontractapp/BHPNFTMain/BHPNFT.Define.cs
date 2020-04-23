using Bhp.SmartContract.Framework;
using System.ComponentModel;
using System.Numerics;

namespace BhpHashPowerNFT
{
    
    /// 事件，变量，结构定义
    
    public partial class HashPowerContract : SmartContract 
    {

        //超级管理員
        static readonly byte[] superAdmin = Helper.ToScriptHash("AWWx2F1Ph9oJtbU8H2mcJGDgDeFDH8geWs");

        #region Storage key prefixes
        
        static readonly string StoragePrefixContractState = "contractState";
        static readonly string StoragePrefixAsset = "asset";
        static readonly string StoragePrefixOwnerNFTList = "ownerNFTList";
        static readonly string StoragePrefixPledgerNFTList = "pledgerNFTList";
        static readonly string StoragePrefixIssuerKeyNFTList = "issuerKeyNFTList";
        static readonly string StoragePrefixIssuerAddrs = "issuerAddrs";
        static readonly string StoragePrefixApproveAll = "approveAll";
        static readonly string StoragePrefixApproveAsset = "approveAsset";
        static readonly string StoragePrefixIssuerKey = "issuerKey";

        static readonly string TotalSupplyMapKey = "totalSupply";
        
        #endregion

        #region 初始事件声明
        
        //铸币事件
        public delegate void deleMint(byte[] owner, BigInteger amount);
        [DisplayName("mint")]
        public static event deleMint onMint;
        public delegate void deleNFTMint(byte[] owner, BigInteger assetId, Asset asset);
        [DisplayName("NFTmint")]
        public static event deleNFTMint onNFTMint;

        //NFT修改事件
        public delegate void deleNFTModify(BigInteger assetId, string elementName, object elementData);
        [DisplayName("NFTModify")]
        public static event deleNFTModify onNFTModify;

        //授权事件
        public delegate void deleApprove(byte[] owner, byte[] approve, BigInteger amount);
        [DisplayName("approve")]
        public static event deleApprove onApprove;
        public delegate void deleNFTApprove(byte[] owner, byte[] approve, BigInteger assetId);
        [DisplayName("NFTapprove")]
        public static event deleNFTApprove onNFTApprove;

        //质押事件
        public delegate void delePledged(byte[] owner, byte[] pledger, BigInteger amount);
        [DisplayName("pledged")]
        public static event delePledged onPledged;
        public delegate void deleNFTPledged(byte[] owner, byte[] pledger, BigInteger assetId);
        [DisplayName("NFTpledgede")]
        public static event deleNFTPledged onNFTPledged;

        //转账事件
        public delegate void deleTransfer(byte[] from, byte[] to, BigInteger amount);
        [DisplayName("transfer")]
        public static event deleTransfer onTransfer;
        public delegate void deleNFTTransfer(byte[] from, byte[] to, BigInteger asset, BigInteger amount);
        [DisplayName("NFTtransfer")]
        public static event deleNFTTransfer onNFTTransfer;
 
        #endregion        

        #region  asset 定义
        public class Asset
        {
            public Asset()
            {
                assetId = 0;
                owner = new byte[0]; //资产拥有
                issuerKey = ""; //发行地址
                pledger = new byte[0]; //质押人地址
                ownershipStartDate = 20200101; //所有权开始日期
                basicHashPowerAmount = 0; //基础算力
                basicHashPowerExpiryDate = 20200101; //基础算力有效日期
                floatingHashPowerAmount = 0; //活期上浮
                floatingHashPowerExpiryDate = 20200101;
                regularHashPowerAmount = 0; //定期上浮
                regularHashPowerExpiryDate = 20200101;
                uptoStdHashPowerAmount = 0; //达标上浮
                uptoStdHashPowerExpiryDate = 20200101;
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
            public string issuerKey; //发行人地址
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
    }
}
