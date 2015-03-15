using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class DataDefinition
    {
        public abstract class TerminalType
        {
            public static string Plasma = "Plasma";
            public static string LCD = "LCD";
            public static string CRT = "CRT";
            public static string LED = "LED";
        }

        public abstract class DisplayPeriod
        {
            public static string Periodically = "P";
            public static string NTimes = "NT";
        }

        public abstract class RepeatTimes
        {
            public static int Once = 1;
            public static int Twice = 2;
            public static int Thrice = 3;
            public static int Ten = 10;
            public static int Month = 30;
            public static int Hundred = 100;
            public static int Year = 365;
        }
    }
}
