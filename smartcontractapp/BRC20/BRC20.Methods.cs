using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System;
using System.Numerics;

namespace BRC20
{
    public partial class BRC20 : SmartContract
    {
        private static BigInteger TotalSupply()
        {
            StorageMap contract = Storage.CurrentContext.CreateMap(StoragePrefixContract);
            return contract.Get("totalSupply")?.ToBigInteger() ?? 0;
        }

        private static BigInteger BalanceOf(byte[] account)
        {
            if (!ValidateAddress(account)) throw new FormatException("The parameter 'account' SHOULD be 20-byte addresses.");

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            return balances.Get(account)?.ToBigInteger() ?? 0;
        }

        private static bool Transfer(byte[] from, byte[] to, BigInteger amount)
        {
            if (!ValidateAddress(from)) throw new FormatException("The parameter 'from' SHOULD be 20-byte addresses.");
            if (!ValidateAddress(to)) throw new FormatException("The parameters 'to' SHOULD be 20-byte addresses.");
            if (!IsPayable(to)) return false;
            if (amount <= 0) throw new InvalidOperationException("The parameter amount MUST be greater than 0.");
            if (!Runtime.CheckWitness(from)) return false;

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            BigInteger fromAmount = balances.Get(from).ToBigInteger();

            if (fromAmount < amount) return false;
            if (amount == 0 || from == to) return true;

            if (fromAmount == amount)
            {
                balances.Delete(from);
            }
            else
            {
                balances.Put(from, fromAmount - amount);
            }

            BigInteger toAmount = balances.Get(to)?.ToBigInteger() ?? 0;
            balances.Put(to, toAmount + amount);

            OnTransfer(from, to, amount);
            return true;
        }

        /// <summary>
        /// 发行资产
        /// </summary>
        /// <param name="to">目标地址</param>
        /// <param name="amount">数量</param>
        /// <returns></returns>
        private static bool Mint(byte[] to, BigInteger amount)
        {
            if (!ValidateAddress(to)) throw new FormatException("The parameters 'to' SHOULD be 20-byte addresses.");
            if (!IsPayable(to)) return false;
            if (amount <= 0) throw new InvalidOperationException("The parameter amount MUST be greater than 0.");

            StorageMap issuerContract = Storage.CurrentContext.CreateMap(StoragePrefixIssuer);
            byte[] issuer = issuerContract.Get("issuer");
            if (issuer == null) throw new FormatException("Set issuer address first.");
            if (!Runtime.CheckWitness(issuer)) return false;

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            BigInteger toAmount = balances.Get(to)?.ToBigInteger() ?? 0;
            balances.Put(to, toAmount + amount);

            StorageMap contract = Storage.CurrentContext.CreateMap(StoragePrefixContract);
            BigInteger totalSupply = contract.Get("totalSupply")?.ToBigInteger() ?? 0;
            contract.Put("totalSupply", totalSupply + amount);

            OnIssue(to, amount);
            return true;
        }

        /// <summary>
        /// 授权to地址操作from地址中的资产，最大数量为amount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="spender"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private static bool Approve(byte[] sender, byte[] spender, BigInteger amount)
        {
            if (!ValidateAddress(sender)) throw new FormatException("The parameter 'from' SHOULD be 20-byte addresses.");
            if (!ValidateAddress(spender)) throw new FormatException("The parameters 'to' SHOULD be 20-byte addresses.");
            if (!IsPayable(spender)) return false;
            if (amount <= 0) throw new InvalidOperationException("The parameter amount MUST be greater than 0.");
            if (!Runtime.CheckWitness(sender)) return false; //只能自己操作

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            BigInteger fromAmount = balances.Get(sender).ToBigInteger();

            if (fromAmount < amount)//可以授权
            {
                //do something ...
                return true;
            }

            if (amount == 0)//取消授权 
            {
                //do something ...
                return true;
            }

            if (sender == spender) return true; //自己授权自己

            StorageMap balancesApprove = Storage.CurrentContext.CreateMap(StoragePrefixApprove);

            //方案一 ，采用from+to作为KEY，存储授权金额，简单
            //问题，如果多次授权不同的地址时，会有存储空间的浪费，
            //可借助于另外一个MAP来记录未被授权的余额，在授权时判断可用余额
            byte[] approveKey = sender.Concat(spender);
            balancesApprove.Put(approveKey, amount);

            /*
            //方案二，采用MAP存储
            //Map序列化是否能正确存储，需要验证             
            Map<byte[], BigInteger> approveMap = new Map<byte[], BigInteger>();
            approveMap[to] = amount;            
            //如果有多次授权，只以最后一次授权金额为准                    
            balancesApprove.Put(from, approveMap.Serialize());             
            */

            //触发事件
            OnApprove(sender, spender, amount);
            return true;
        }

        /// <summary>
        /// 从授权地址转账
        /// </summary>
        /// <param name="spender">操作地址</param>
        /// <param name="from">源地址</param>
        /// <param name="to">目的地址</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        private static bool TransferFrom(byte[] spender, byte[] from, byte[] to, BigInteger amount)
        {
            if (!ValidateAddress(from)) throw new FormatException("The parameter 'from' SHOULD be 20-byte addresses.");
            if (!ValidateAddress(to)) throw new FormatException("The parameters 'to' SHOULD be 20-byte addresses.");
            if (!IsPayable(to)) return false;
            if (amount <= 0) throw new InvalidOperationException("The parameter amount MUST be greater than 0.");
            if (!Runtime.CheckWitness(spender)) return false; //只能被授权者才能操作

            StorageMap balancesApprove = Storage.CurrentContext.CreateMap(StoragePrefixApprove);

            //取授权的金额，采用方案一
            byte[] approveKey = from.Concat(spender);
            BigInteger amountOfApprove = balancesApprove.Get(approveKey)?.ToBigInteger() ?? 0;

            if (amountOfApprove < amount) return false; //授权余额不足

            //from余额
            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            BigInteger fromAmount = balances.Get(from).ToBigInteger();

            if (amount == 0 || from == to) return true;

            //处理from余额
            if (fromAmount == amount)
            {
                balances.Delete(from);
            }
            else
            {
                balances.Put(from, fromAmount - amount);
            }

            //处理to余额
            BigInteger toAmount = balances.Get(to)?.ToBigInteger() ?? 0;
            balances.Put(to, toAmount + amount);

            //处理授权余额
            if (amountOfApprove == amount)
            {
                balancesApprove.Delete(approveKey);
            }
            else
            {
                balancesApprove.Put(approveKey, amountOfApprove - amount);
            }

            //触发事件
            OnTransferFrom(spender, from, to, amount);
            return true;
        }
    }
}
