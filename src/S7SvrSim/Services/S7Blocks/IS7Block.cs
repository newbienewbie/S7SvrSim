namespace S7SvrSim.Services.S7Blocks;

public interface IS7Block
{
    bool ReadBit(int offset, byte bit);
    void WriteBit(int offset, byte bit, bool flag);

    byte ReadByte(int pos);
    void WriteByte(int pos, byte value);

    short ReadShort(int pos);
    void WriteShort(int pos, short value);

    ushort ReadUShort(int pos);
    void WriteUShort(int pos, ushort value);

    uint ReadUInt32(int pos);
    void WriteUInt32(int pos, uint value);

    ulong ReadULong(int pos);
    void WriteULong(int pos, ulong value);

    float ReadReal(int pos);
    void WriteReal(int pos, float value);

    double ReadLReal(int pos);
    void WriteLReal(int pos, double value);

    string ReadString(int offset);
    void WriteString(int offset, int maxlen, string str);
    int ReadDInt(int pos);
    void WriteDInt(int pos, int value);
}
