using System.IO;
using System.Text;

namespace S7SvrSim.Shared
{
    internal static class StreamExtensions
    {
        public static void WriteString(this Stream stream, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            stream.Write(Encoding.UTF8.GetBytes(value));
        }
    }
}
