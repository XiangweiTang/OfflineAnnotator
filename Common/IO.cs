using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public static class IO
    {
        public static string ReadBytesToString(this Stream st, int count, Encoding encoding)
        {
            byte[] bytes = new byte[count];
            st.Read(bytes, 0, count);
            return encoding.GetString(bytes);
        }
        public static int ReadBytesToInt32(this Stream st)
        {
            byte[] bytes = new byte[4];
            st.Read(bytes, 0, 4);
            return BitConverter.ToInt32(bytes, 0);
        }
        public static short ReadBytesToInt16(this Stream st)
        {
            byte[] bytes = new byte[2];
            st.Read(bytes, 0, 2);
            return BitConverter.ToInt16(bytes, 0);
        }
        public static byte ReadBytesToByte(this Stream st)
        {
            return (byte)st.ReadByte();
        }
    }
}
