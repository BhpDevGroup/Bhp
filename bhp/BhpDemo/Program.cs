using Bhp.Cryptography;
using Bhp.Wallets;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace BhpDemo
{
    public class Program
    {
        static void CreateAddress()
        {
            byte[] privateKey = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(privateKey);
            }
            KeyPair keyPair = new KeyPair(privateKey);
            keyPair.Export(); 

            Console.WriteLine();
            //Console.WriteLine($"PrivateKey: {keyPair.GetPrivateKeyHex()}"); 
            Console.WriteLine($"PublicKey: {keyPair.PublicKey.ToString()}");
            Console.WriteLine($"Address: {keyPair.PublicKeyHash.ToAddress() }");
        } 

        static void CreateAddressList(int count)
        {
            string path = $"{Directory.GetCurrentDirectory()}\\address.txt";

            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs); 

            for (int i = 0; i < count; i++)
            {
                byte[] privateKey = new byte[32];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(privateKey);
                }
                KeyPair keyPair = new KeyPair(privateKey);

                //开始写入
                sw.WriteLine($"{BytesToHex(keyPair.PrivateKey)} {keyPair.PublicKey.ToString()} {keyPair.PublicKeyHash.ToAddress()}");
                //清空缓冲区
                sw.Flush();
                //keyPair.Export(); 
               
                //Console.WriteLine($"PrivateKey: {keyPair.GetPrivateKeyHex()}"); 
                Console.WriteLine($"Index: {i} PublicKey: {keyPair.PublicKey.ToString()} Address: {keyPair.PublicKeyHash.ToAddress() }");
             }

            //关闭流
            sw.Close();
            fs.Close();
        }

        public static byte[] HexToBytes(string value)
        {
            if (value == null || value.Length == 0)
                return new byte[0];
            if (value.Length % 2 == 1)
                throw new FormatException();
            byte[] result = new byte[value.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = byte.Parse(value.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
            return result;
        } 

        public static string BytesToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes, 0).Replace("-", string.Empty).ToLower();
        }

        static void SignR1()
        {
            Console.WriteLine("\n------SignR1------");
            //    byte[] privateKey = new byte[32];
            //    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            //    {
            //        rng.GetBytes(privateKey);
            //    }
            //AWgC4j6Pdamq77nfbgfoxhRYb1vXpPsSgn
            byte[] message = Encoding.ASCII.GetBytes("Hello World");
            byte[] privateKey = HexToBytes("af401a6c2aa77cffe7b7e80e597023699d3f4a793c94b25effd686502331cd5b");
            //byte[] publicKey = HexToBytes("03a935e27bbe57c45352dcdc78485f0652018567cc24c6832014a751303922ea97");

            KeyPair keyPair = new KeyPair(privateKey); 
            byte[] signature = CryptoR1.Default.Sign(message, privateKey,
                keyPair.PublicKey.EncodePoint(false).Skip(1).ToArray());

            string hex = BitConverter.ToString(signature, 0).Replace("-", string.Empty).ToLower();
            Console.WriteLine("signature: " + hex);

            bool verify = CryptoR1.Default.VerifySignature(message, signature, keyPair.PublicKey.EncodePoint(false).Skip(1).ToArray());
            Console.WriteLine("verify: " + verify);

            byte[] privateKey2 = HexToBytes("b5216a93b7996a2cc56451f18dd363dccc61d22ad0647d4bb164d62d028bef46");
            KeyPair key = new KeyPair(privateKey2);
            verify = CryptoR1.Default.VerifySignature(message, signature, key.PublicKey.EncodePoint(false).Skip(1).ToArray());
            Console.WriteLine("verify: " + verify);
        }

        static void SignK1()
        {
            Console.WriteLine("\n           ------SignK1------");
         
            byte[] message = Encoding.ASCII.GetBytes("Hellod大是大非加工别人 World阿斯蒂芬12312宽松234东奔西走 AADDFFSD");
            byte[] privateKey1 = HexToBytes("05d08feb9b115375e052a09cef9d2410b44281cd7a4f32ad7f202ab91d38eba4");
            //byte[] publicKey = HexToBytes("020ecf04ed46f6f704e449c4807ccad1107b88cebe9a507aaa0357ee0e4b41e65e");
            //string address = "AaMwQzpxzU6EUNoThZL8n33AQThXh2DCpS";

            KeyPair keyPair1 = new KeyPair(privateKey1);
            //BigInteger[] 
            //BigInteger[] signature = Crypto.Default.Sign(message, privateKey1);
            byte[] signature = Crypto.Default.Sign(message, privateKey1, keyPair1.PublicKey.EncodePoint(false).Skip(1).ToArray());
            //string hex = BitConverter.ToString(signature, 0).Replace("-", string.Empty).ToLower();
            //Console.WriteLine($"    signature:{BytesToHex(signature[0].ToByteArray())}-{BytesToHex(signature[1].ToByteArray())} ");
            Console.WriteLine($"    signature:{BytesToHex(signature)} ");

            bool verify = Crypto.Default.VerifySignature(message, signature, keyPair1.PublicKey.EncodePoint(false));
            Console.WriteLine($"    publicKey: { keyPair1.PublicKeyHash.ToString()} verify: {verify}");

            //Test Verify
            WritFile("Starting Test Verify......");
            for (int i = 0; i < 1000000; i++)
            {
                keyPair1 = new KeyPair(privateKey1);
                verify = Crypto.Default.VerifySignature(message, signature, keyPair1.PublicKey.EncodePoint(false));
                Console.WriteLine($" True  index:{i + 1},publicKey: { keyPair1.PublicKeyHash.ToString()} verify: {verify}");

                byte[] privateKey2 = new byte[32];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(privateKey2);
                }
                KeyPair keyPair2 = new KeyPair(privateKey2); 
                verify = Crypto.Default.VerifySignature(message, signature, keyPair2.PublicKey.EncodePoint(false));

                if (verify)
                {
                    WritFile($" False index:{i + 1},privateKey: {BytesToHex(privateKey2)}, publicKey:{ keyPair2.PublicKeyHash.ToString()}  verify: {verify}");
                }
                Console.WriteLine($" False index:{i+1},publicKey: { keyPair2.PublicKeyHash.ToString()} verify: {verify}");
            }
         
            WritFile("The end of Test Verify");
               
        }

        static void WritFile(string msg)
        {
            string path = $"{Directory.GetCurrentDirectory()}\\log.txt";
  
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.WriteLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}: {msg}");
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        

        public static BigInteger HexToBigInteger(string hex)
        {
            byte[] bytes = HexToBytes(hex);
            Array.Reverse(bytes);
            Array.Resize(ref bytes, bytes.Length + 1);
            bytes[bytes.Length - 1] = 0x00;
            return new BigInteger(bytes);
        }

        static void Main(string[] args)
        {
            //SignR1();
            SignK1();

            //CreateAddressList(100000);
            Console.ReadLine();
        }
    }
}
