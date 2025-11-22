using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandCaller.App.Models
{
    public static class Log
    {
        public static void WriteLog(string filename, string type, string message)
        {
            Console.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Console.Write(" | ");
            switch(type)
            {
                case "Info":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("Info");
                    break;
                case "Success":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Succ");
                    break;
                case "Warn":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Warn");
                    break;
                case "Error":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Erro");
                    break;
                case "Fail":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("Fail");
                    break;
                case "Debug":
                    Console.Write("Dbug");
                    break;
                default:
                    Console.Write("None");
                    break;
            }
            Console.ResetColor();
            Console.Write(" | ");
            Console.ForegroundColor= ConsoleColor.Magenta;
            Console.Write(filename);
            Console.ResetColor();
            Console.WriteLine(" : " + message);
        }
    }
}
