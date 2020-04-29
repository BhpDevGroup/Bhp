using Bhp.SmartContract.Framework;
using Bhp.SmartContract.Framework.Services.Bhp;
using System;
using System.Numerics;

namespace IteratorProcessingContract
{
    public class IteratorProcessing : SmartContract
    {
        delegate object deleDyncall(string method, object[] args);

        public static object Main(string contractHash, BigInteger startIndex, BigInteger count, string operation, object[] args)
        {
            if (Runtime.Trigger == TriggerType.Application)
            {
                if (startIndex < 0) throw new InvalidOperationException("The parameter startIndex cannot be less than 0.");
                if (count < 1 || count > 1000) throw new InvalidOperationException("The parameter count must be between 1 and 1000.");

                byte[] contract = contractHash.AsByteArray();
                deleDyncall dyncall = (deleDyncall)contract.ToDelegate();

                Iterator<byte[], byte[]> iteratorResult = (Iterator<byte[], byte[]>)dyncall(operation, args);
                Map<byte[], byte[]> map = new Map<byte[], byte[]>();
                BigInteger currentIndex = 0;
                BigInteger endIndex = startIndex + count;
                while (iteratorResult.Next())
                {
                    if (currentIndex >= startIndex && currentIndex < endIndex)
                    {
                        map[iteratorResult.Key] = iteratorResult.Value;
                    }
                    currentIndex += 1;
                }
                return map;
            }
            return false;
        }
    }
}
