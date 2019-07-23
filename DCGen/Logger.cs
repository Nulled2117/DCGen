using System;

namespace DCGen
{
    public enum LogMode
    {
        Error,
        Info,
        Success
    }

    public static class Logger
    {
        

        public static void Log(string msg, LogMode mode)
        {
            switch(mode)
            {
                case LogMode.Info:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogMode.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogMode.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(string.Format("[DCGen] {0}", msg));
        }
    }
}
