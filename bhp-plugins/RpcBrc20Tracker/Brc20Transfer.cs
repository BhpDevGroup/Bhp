using System.IO;
using System.Numerics;
using Bhp.IO;
using Bhp.Ledger;

namespace Bhp.Plugins
{
    public class Brc20Transfer : StateBase, ICloneable<Brc20Transfer>
    {
        public UInt160 UserScriptHash;
        public uint BlockIndex;
        public UInt256 TxHash;
        public BigInteger Amount;

        public override int Size => base.Size + 20 + sizeof(uint) + 32 + Amount.ToByteArray().GetVarSize();

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UserScriptHash);
            writer.Write(BlockIndex);
            writer.Write(TxHash);
            writer.WriteVarBytes(Amount.ToByteArray());
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            UserScriptHash = reader.ReadSerializable<UInt160>();
            BlockIndex = reader.ReadUInt32();
            TxHash = reader.ReadSerializable<UInt256>();
            Amount = new BigInteger(reader.ReadVarBytes(512));
        }

        public Brc20Transfer Clone()
        {
            return new Brc20Transfer
            {
                UserScriptHash = UserScriptHash,
                BlockIndex = BlockIndex,
                TxHash = TxHash,
                Amount = Amount
            };
        }

        public void FromReplica(Brc20Transfer replica)
        {
            UserScriptHash = replica.UserScriptHash;
            BlockIndex = replica.BlockIndex;
            TxHash = replica.TxHash;
            Amount = replica.Amount;
        }
    }
}