using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Sanity
    {
        public static void Requires(bool b, string message, int hResult = -1)
        {
            if (!b)
            {
                throw new CommonException(message, hResult);
            }
        }

        public static void Requires(bool b, int hResult = -1)
        {
            if (!b)
            {
                throw new CommonException(hResult); 
            }
        }
    }

    public class CommonException : Exception
    {        
        public CommonException(int hResult = -1) : base()
        {
            HResult = hResult;
        }
        public CommonException(string message, int hResult = -1) : base(message)
        {
            HResult = hResult;
        }
    }
}
