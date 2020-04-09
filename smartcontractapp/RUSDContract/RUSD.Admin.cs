using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using Bhp.SmartContract.Framework.Services.System;
using System;

namespace RUSDContract
{
    public partial class RUSD : SmartContract
    {
        /// <summary>
        /// 初始化资产
        /// </summary>
        /// <returns>true:初始化成功, false:初始化失败</returns>
        private static bool Deploy()
        {
            if (!Runtime.CheckWitness(Owner)) return false;

            if (InitialSupply <= 0) return false;

            StorageMap contract = Storage.CurrentContext.CreateMap(StoragePrefixContract);
            if (contract.Get("totalSupply") != null)
                throw new Exception("Contract already deployed");

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            balances.Put(Owner, InitialSupply);
            contract.Put("totalSupply", InitialSupply);

            OnTransfer(null, Owner, InitialSupply);
            return true;
        }

        /// <summary>
        /// 合约升级
        /// </summary>
        /// <param name="args">合约参数</param>
        /// args[0]:新合约脚本
        /// args[1]:输入参数
        /// args[2]:返回类型
        /// args[3]:属性, 无属性:0, 存储区:1 << 0, 动态调用:1 << 1, 可支付:1 << 2 -- [ex: 存储区+动态调用 -> 11 -> 3, ex: 存储区+动态调用+可支付 -> 111 -> 7]
        /// args[4]:名称
        /// args[5]:版本
        /// args[6]:作者
        /// args[7]:邮箱
        /// args[8]:描述
        /// <returns>true:升级成功, false:升级失败</returns>
        public static bool Migrate(object[] args)
        {
            if (!Runtime.CheckWitness(Owner))
                return false;

            if (args.Length < 9) return false;

            byte[] script = Blockchain.GetContract(ExecutionEngine.ExecutingScriptHash).Script;
            byte[] new_script = (byte[])args[0];

            if (new_script.Length == 0) return false;

            if (script == new_script) return false;

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

        private static bool Migrate(byte[] script, byte[] plist, byte rtype, ContractPropertyState cps, string name, string version, string author, string email, string description)
        {
            var contract = Contract.Migrate(script, plist, rtype, cps, name, version, author, email, description);
            return true;
        }

        /// <summary>
        /// 销毁合约
        /// </summary>
        /// <returns>true:销毁成功, false:销毁失败</returns>
        public static bool Destroy()
        {
            return false;
        }

        /// <summary>
        /// 设置铸币权限
        /// </summary>
        /// <param name="mintAddr">被授权的铸币地址</param>
        /// <returns>true:设置成功, false:设置失败</returns>
        public static bool SetMintAddr(byte[] mintAddr)
        {
            if (!Runtime.CheckWitness(Owner)) return false;
            if (!ValidateAddress(mintAddr)) throw new FormatException("The parameters 'to' SHOULD be 20-byte addresses.");

            StorageMap contract = Storage.CurrentContext.CreateMap(StoragePrefixMintAddr);
            contract.Put("mintAddr", mintAddr);
            return true;
        }
    }
}
