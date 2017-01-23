using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public enum Logtype { Info, Warning, Error }
    static class Logger
    {
        static void Log(string message, Logtype type = Logtype.Info)
        {
            Console.ForegroundColor = ConsoleColor.White;
            if(type != Logtype.Info)
            {
                if (type == Logtype.Warning)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine(message);
        }

        static void LogError(string message)
        {
            Log(message, Logtype.Error);
        }

        static void LogWarning(string message)
        {
            Log(message, Logtype.Warning);
        }
    }
}
