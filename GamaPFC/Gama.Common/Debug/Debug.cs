using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Common.Debug
{
    public static class Debug
    {
        public static bool DEBUG = true;

        private static System.Diagnostics.Stopwatch _Stopwatch;

        public static void StartWatch()
        {
            if (DEBUG)
                _Stopwatch = System.Diagnostics.Stopwatch.StartNew();
        }

        public static void StopWatch(string source)
        {
            if (DEBUG)
            {
                _Stopwatch.Stop();
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                Console.WriteLine($">>>>>>>>>>>>>>{source}: {_Stopwatch.ElapsedMilliseconds / 1000.0} segundos");
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            }
        }
    }
}
