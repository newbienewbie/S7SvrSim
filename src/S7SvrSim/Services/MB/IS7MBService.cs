namespace S7Svr.Simulator.ViewModels
{
    public interface IS7MBService
    {
        bool ReadBit(int offset, byte bit);
        void WriteBit(int offset, byte bit, bool flag);

        byte ReadByte(int pos);
        void WriteByte(int pos, byte value);

        short ReadShort(int pos);
        void WriteShort(int pos, short value);

        uint ReadUInt32(int pos);
        void WriteUInt32(int pos, uint value);

        ulong ReadULong(int pos);
        void WriteULong(int pos, ulong value);

        float ReadReal(int pos);
        void WriteReal(int pos, float real);

        string ReadString(int offset);
        void WriteString(int offset, int maxlen, string str);
    }
}