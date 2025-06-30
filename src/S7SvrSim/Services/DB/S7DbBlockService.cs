using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.Services.DB
{
    public class S7DbBlockService : IS7Block
    {
        private readonly IS7DataBlockService db;
        private readonly int dbIndex;

        public S7DbBlockService(IS7DataBlockService db, int dbIndex)
        {
            this.db = db;
            this.dbIndex = dbIndex;
        }

        public bool ReadBit(int offset, byte bit)
        {
            return db.ReadBit(dbIndex, offset, bit);
        }

        public byte ReadByte(int pos)
        {
            return db.ReadByte(dbIndex, pos);
        }

        public int ReadInt(int pos)
        {
            return db.ReadInt(dbIndex, pos);
        }

        public double ReadLReal(int pos)
        {
            return db.ReadLReal(dbIndex, pos);
        }

        public float ReadReal(int pos)
        {
            return db.ReadReal(dbIndex, pos);
        }

        public short ReadShort(int pos)
        {
            return db.ReadShort(dbIndex, pos);
        }

        public string ReadString(int offset)
        {
            return db.ReadString(dbIndex, offset);
        }

        public uint ReadUInt32(int pos)
        {
            return db.ReadUInt32(dbIndex, pos);
        }

        public ulong ReadULong(int pos)
        {
            return db.ReadULong(dbIndex, pos);
        }

        public ushort ReadUShort(int pos)
        {
            return db.ReadUShort(dbIndex, pos);
        }

        public void WriteBit(int offset, byte bit, bool flag)
        {
            db.WriteBit(dbIndex, offset, bit, flag);
        }

        public void WriteByte(int pos, byte value)
        {
            db.WriteByte(dbIndex, pos, value);
        }

        public void WriteInt(int pos, int value)
        {
            db.WriteInt(dbIndex, pos, value);
        }

        public void WriteLReal(int pos, double value)
        {
            db.WriteLReal(dbIndex, pos, value);
        }

        public void WriteReal(int pos, float value)
        {
            db.WriteReal(dbIndex, pos, value);
        }

        public void WriteShort(int pos, short value)
        {
            db.WriteShort(dbIndex, pos, value);
        }

        public void WriteString(int offset, int maxlen, string str)
        {
            db.WriteString(dbIndex, offset, maxlen, str);
        }

        public void WriteUInt32(int pos, uint value)
        {
            db.WriteUInt32(dbIndex, pos, value);
        }

        public void WriteULong(int pos, ulong value)
        {
            db.WriteULong(dbIndex, pos, value);
        }

        public void WriteUShort(int pos, ushort value)
        {
            db.WriteUShort(dbIndex, pos, value);
        }
    }
}
