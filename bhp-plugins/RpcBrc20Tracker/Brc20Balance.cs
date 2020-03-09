using System.IO;
using System.Numerics;
using Bhp.IO;
using Bhp.Ledger;

namespace Bhp.Plugins
{
    public class Brc20Balance : StateBase, ICloneable<Brc20Balance>
    {
        public BigInteger Balance;
        public uint LastUpdatedBlock;

        public override int Size => base.Size + Balance.ToByteArray().GetVarSize() + sizeof(uint);

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.WriteVarBytes(Balance.ToByteArray());
            writer.Write(LastUpdatedBlock);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            Balance = new BigInteger(reader.ReadVarBytes(512));
            LastUpdatedBlock = reader.ReadUInt32();
        }

        public Brc20Balance Clone()
        {
            return new Brc20Balance
            {
                Balance = Balance,
                LastUpdatedBlock = LastUpdatedBlock
            };
        }

        public void FromReplica(Brc20Balance replica)
        {
            Balance = replica.Balance;
            LastUpdatedBlock = replica.LastUpdatedBlock;
        }
    }
}