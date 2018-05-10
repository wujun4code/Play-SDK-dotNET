using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud
{
    public static class PlayLogger
    {
        static PlayLogger()
        {
            Play.Logger = Console.WriteLine;
            Play.ErrorLogger = Console.WriteLine;
        }

        internal static void Open()
        {
            Play.Logger = Console.WriteLine;
            Play.ErrorLogger = Console.WriteLine;
        }

        internal static void Close()
        {
            Play.Logger = null;
            Play.ErrorLogger = null;
        }
    }
}
