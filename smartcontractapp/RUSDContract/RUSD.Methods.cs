using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System;
using System.Numerics;

namespace RUSDContract
{
    public partial class RUSD : SmartContract
    {
        /// <summary>
        /// 获取已发行资产
        /// </summary>
        /// <returns>已发行资产金额</returns>
        private static BigInteger TotalSupply()
        {
            StorageMap contract = Storage.CurrentContext.CreateMap(StoragePrefixContract);
            return contract.Get("totalSupply").ToBigInteger();
        }

        /// <summary>
        /// 获取地址拥有的资产金额
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>地址拥有的资产金额</returns>
        private static BigInteger BalanceOf(byte[] address)
        {
            if (!ValidateAddress(address)) throw new FormatException("The parameter 'address' SHOULD be 20-byte addresses.");

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            return balances.Get(address).ToBigInteger();
        }

        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="from">输入地址</param>
        /// <param name="to">输出地址</param>
        /// <param name="amount">转账金额</param>
        /// <returns>true:转账成功, false:转账失败</returns>
        private static bool Transfer(byte[] from, byte[] to, BigInteger amount)
        {
            if (!ValidateAddress(from)) throw new FormatException("The parameter 'from' SHOULD be 20-byte addresses.");
            if (!ValidateAddress(to)) throw new FormatException("The parameters 'to' SHOULD be 20-byte addresses.");
            if (!IsPayable(to)) return false;
            if (amount <= 0) throw new InvalidOperationException("The parameter amount MUST be greater than 0.");
            if (!Runtime.CheckWitness(from)) return false;

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            BigInteger fromAmount = balances.Get(from).ToBigInteger();

            if (fromAmount < amount) return false;//余额不足
            if (amount == 0 || from == to) return true;//无需操作存储区

            if (fromAmount == amount)
            {
                balances.Delete(from);
            }
            else
            {
                balances.Put(from, fromAmount - amount);
            }

            BigInteger toAmount = balances.Get(to).ToBigInteger();
            balances.Put(to, toAmount + amount);

            OnTransfer(from, to, amount);
            return true;
        }

        /// <summary>
        /// 铸币
        /// </summary>
        /// <param name="to">目标地址</param>
        /// <param name="amount">数量</param>
        /// <returns>true:铸币成功, false:铸币失败</returns>
        private static bool Mint(byte[] to, BigInteger amount)
        {
            if (!ValidateAddress(to)) throw new FormatException("The parameters 'to' SHOULD be 20-byte addresses.");
            if (!IsPayable(to)) return false;
            if (amount <= 0) throw new InvalidOperationException("The parameter amount MUST be greater than 0.");

            //验证铸币地址
            StorageMap mintContract = Storage.CurrentContext.CreateMap(StoragePrefixMintAddr);
            byte[] mintAddr = mintContract.Get("mintAddr");
            if (mintAddr == null) throw new FormatException("Set mint address first.");
            if (!Runtime.CheckWitness(mintAddr)) return false;

            //铸币
            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            BigInteger toAmount = balances.Get(to).ToBigInteger();
            balances.Put(to, toAmount + amount);

            //更新发行总量
            StorageMap contract = Storage.CurrentContext.CreateMap(StoragePrefixContract);
            BigInteger totalSupply = contract.Get("totalSupply").ToBigInteger();
            contract.Put("totalSupply", totalSupply + amount);

            OnMint(to, amount);
            return true;
        }

        /// <summary>
        /// 授权spender地址操作sender地址中的资产，最大数量为amount
        /// </summary>
        /// <param name="sender">拥有者地址</param>
        /// <param name="spender">被授权者地址</param>
        /// <param name="amount">授权金额</param>
        /// <returns>true:授权成功, false:授权失败</returns>
        private static bool Approve(byte[] sender, byte[] spender, BigInteger amount)
        {
            if (!ValidateAddress(sender)) throw new FormatException("The parameter 'sender' SHOULD be 20-byte addresses.");
            if (!ValidateAddress(spender)) throw new FormatException("The parameters 'spender' SHOULD be 20-byte addresses.");
            if (!IsPayable(spender)) return false;

            if (amount < 0) throw new InvalidOperationException("The parameter amount cannot be less than 0.");
            if (!Runtime.CheckWitness(sender)) return false; //只能自己操作
            if (sender == spender) return true; //自己授权自己

            StorageMap balancesApprove = Storage.CurrentContext.CreateMap(StoragePrefixApprove);

            if (amount == 0)//取消授权
            {
                byte[] fromApprove = balancesApprove.Get(sender);
                Map<byte[], BigInteger> spenderMap = (Map<byte[], BigInteger>)fromApprove.Deserialize();
                if (spenderMap.HasKey(spender))
                {
                    balancesApprove.Delete(sender);
                }
            }
            else
            {
                Map<byte[], BigInteger> newSpenderMap = new Map<byte[], BigInteger>();
                newSpenderMap[spender] = amount;
                balancesApprove.Put(sender, newSpenderMap.Serialize());
            }

            //触发事件
            OnApprove(sender, spender, amount);
            return true;
        }

        /// <summary>
        /// 从授权地址转账
        /// </summary>
        /// <param name="spender">操作地址</param>
        /// <param name="sender">源地址</param>
        /// <param name="to">目的地址</param>
        /// <param name="amount">金额</param>
        /// <returns>true:授权地址转账成功, false:授权地址转账失败</returns>
        private static bool TransferFrom(byte[] spender, byte[] sender, byte[] to, BigInteger amount)
        {
            if (!ValidateAddress(spender)) throw new FormatException("The parameter 'spender' SHOULD be 20-byte addresses.");
            if (!ValidateAddress(sender)) throw new FormatException("The parameter 'sender' SHOULD be 20-byte addresses.");
            if (!ValidateAddress(to)) throw new FormatException("The parameters 'to' SHOULD be 20-byte addresses.");
            if (!IsPayable(to)) return false;
            if (amount <= 0) throw new InvalidOperationException("The parameter amount MUST be greater than 0.");
            if (!Runtime.CheckWitness(spender)) return false; //只能被授权者才能操作

            StorageMap balancesApprove = Storage.CurrentContext.CreateMap(StoragePrefixApprove);

            byte[] fromApprove = balancesApprove.Get(sender);
            if (fromApprove == null) return false;

            Map<byte[], BigInteger> spenderMap = (Map<byte[], BigInteger>)fromApprove.Deserialize();
            if (!spenderMap.HasKey(spender)) return false;//只能被授权者才能操作

            BigInteger amountOfApprove = spenderMap[spender];
            if (amountOfApprove < amount) return false; //授权余额不足

            //from余额
            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            BigInteger fromAmount = balances.Get(sender).ToBigInteger();

            if (fromAmount < amount) return false;//余额不足                     

            //sender==to时，转账给自己不更新自己余额，但更新授权余额
            if (sender != to)
            {
                //处理from余额
                if (fromAmount == amount)
                {
                    balances.Delete(sender);
                }
                else
                {
                    balances.Put(sender, fromAmount - amount);
                }

                //处理to余额
                BigInteger toAmount = balances.Get(to).ToBigInteger();
                balances.Put(to, toAmount + amount);
            }

            //处理授权余额
            if (amountOfApprove == amount)
            {
                balancesApprove.Delete(sender);
            }
            else
            {
                spenderMap[spender] = amountOfApprove - amount;
                balancesApprove.Put(sender, spenderMap.Serialize());
            }

            //触发事件
            OnTransferFrom(spender, sender, to, amount);
            return true;
        }

        /// <summary>
        /// 销毁自己的资产
        /// </summary>
        /// <param name="destroyAddr">需要销毁的地址</param>
        /// <param name="amount">需要销毁的金额</param>
        /// <returns>true:销毁成功, false:销毁失败</returns>
        public static bool DestroyAsset(byte[] destroyAddr, BigInteger amount)
        {
            if (!ValidateAddress(destroyAddr)) throw new FormatException("The parameter 'destroyAddr' SHOULD be 20-byte addresses.");
            if (!Runtime.CheckWitness(destroyAddr)) return false; //只能自己能操作

            if (amount <= 0) throw new InvalidOperationException("The parameter amount MUST be greater than 0.");

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            BigInteger ownedAmount = balances.Get(destroyAddr).ToBigInteger();

            if (ownedAmount < amount) return false;

            if (ownedAmount == amount)
            {
                balances.Delete(destroyAddr);
            }
            else
            {
                balances.Put(destroyAddr, ownedAmount - amount);
            }

            OnDestroyAsset(destroyAddr, amount);
            return true;
        }
    }
}
