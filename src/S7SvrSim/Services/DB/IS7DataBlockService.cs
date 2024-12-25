using System.Threading.Tasks;

namespace S7Svr.Simulator.ViewModels
{
    public interface IS7DataBlockService
    {
        bool ReadBit(int dbNumber, int offset, byte bit);
        void WriteBit(int dbNumber, int offset, byte bit, bool flag);

        byte ReadByte(int dbNumber, int pos);
        void WriteByte(int dbNumber, int pos, byte value);

        short ReadShort(int dbNumber, int pos);
        void WriteShort(int dbNumber, int pos, short value);

        uint ReadUInt32(int dbNumber, int pos);
        void WriteUInt32(int dbNumber, int pos, uint value);

        ulong ReadULong(int dbNumber, int pos);
        void WriteULong(int dbNumber, int pos, ulong value);

        float ReadReal(int dbNumber, int pos);
        void WriteReal(int dbNumber, int pos, float real);

        double ReadLReal(int dbNumber, int pos);
        void WriteLReal(int dbNumber, int pos, double real);

        string ReadString(int dbNumber, int offset);
        void WriteString(int dbNumber, int offset, int maxlen, string str);
    }
}