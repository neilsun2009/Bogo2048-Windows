using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048
{
    class Data
    {
        public static int[] data = new int[16] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
        public static int score = 0;

        public static void clear() {
            score = 0;
            for (int i = 0; i< 16; ++i)
            {
                data[i] = 0;
            }
        }
    }
}
