using Bhp.SmartContract.Framework;
using System.Numerics;

namespace StorageContract
{
    public class ObjectMap
    {
        public AddressData[] addressData;
        public static ObjectMap Deserialize(byte[] value)
        {
            ObjectMap objectMap = null;
            if (value != null && value.Length != 0)
            {
                var obj = value.Deserialize();
                objectMap = (ObjectMap)obj;
            }
            return objectMap;
        }
    }

    public class AddressData
    {
        public string address;
        public BasicData[] basic;
        public BasicData[] advanced;

        public static AddressData GetAddressData(ObjectMap value, string address)
        {
            if (value == null || value.addressData == null || value.addressData.Length == 0) return null;
            for (int i = 0; i < value.addressData.Length; i++)
            {
                if (value.addressData[i].address == address)
                {
                    return value.addressData[i];
                }
            }
            return null;
        }

        public static BasicData[] GetBasicData(AddressData value, AccountType type)
        {
            if (value == null) return null;
            BasicData[] basicData = null;
            switch (type)
            {
                case AccountType.Basic:
                    if (value.basic != null && value.basic.Length > 0)
                    {
                        basicData = value.basic;
                    }
                    break;
                case AccountType.Advance:
                    if (value.advanced != null && value.advanced.Length > 0)
                    {
                        basicData = value.advanced;
                    }
                    break;
            }
            return basicData;
        }
    }

    public class BasicData
    {
        public uint time;
        public BigInteger value;

        public static BasicData GetByTime(BasicData[] data, uint time)
        {
            if (data == null || data.Length == 0) return null;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].time == time)
                {
                    return data[i];
                }
            }
            return null;
        }        
    }

    public enum AccountType
    {
        Basic = 1,
        Advance = 2
    }
}
