using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deep_Learning_for_Binary_Options
{
    class Program
    {
        static void Main(string[] args)
        {
            //AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            if (args.Length == 0)
            {
                ShowHelp();
            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "learn":
                        if (args.Length != 3)
                        {
                            ShowHelp();
                            break;
                        }
                        else
                        {
                            Learner.Learn(args[1], args[2]);
                            break;
                        }

                    default:
                        ShowHelp();
                        break;
                }
            }
        }

        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.White;

            Console.WriteLine(e.ExceptionObject.ToString());

            Console.ResetColor();

            Process.GetCurrentProcess().Kill();
        }

        private static void ShowHelp()
        {
            string helpText =
                "Deep Learning for Binary Options" + "\r\n" +
                "\tlearn <parser> <file>";

            Console.WriteLine(helpText);
        }

        public static void Pause()
        {
            string title = Console.Title;
            Console.Title = title + " [Pausiert]";

            while (Console.ReadKey().KeyChar != 'p')
            {
                Thread.Sleep(100);
            }

            Console.Title = title;
        }
    }
}
