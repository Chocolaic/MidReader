using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidReader
{
    class Debug
    {
        public static void printArray(byte[] b)
        {
            Console.WriteLine("{"+string.Join(", ", b)+"}");
        }
    }
}
