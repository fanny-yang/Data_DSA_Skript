using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace finances_optimize
{
    class Program
    {
        /// <summary>
        /// Programmeinstiegspunkt
        /// </summary>
        /// <param name="args">Kommandozeilenargumente</param>
        static void Main(string[] args)
        {
            ConsoleColor backgroundColor = Console.BackgroundColor;
            ConsoleColor foregroundColor = Console.ForegroundColor;

            Console.BackgroundColor = backgroundColor; // ungenutzt
            Console.ForegroundColor = ConsoleColor.Green;

            ShowInitText();

            try
            {
                Dictionary<string, string> rawArguments = new Dictionary<string, string>();

                for (int argIdx = 0; argIdx < args.Length; argIdx++)
                {
                    if (args[argIdx].StartsWith("--"))
                    {
                        rawArguments.Add(args[argIdx], args[argIdx + 1]);
                    }
                    else if (args[argIdx].StartsWith("-"))
                    {
                        rawArguments.Add(args[argIdx], null);
                    }
                }

                if (!rawArguments.ContainsKey("--in"))
                {
                    throw new Exception("Keine Eingabedatei (--in) definiert!");
                }

                if (!rawArguments.ContainsKey("--out"))
                {
                    rawArguments.Add("--out", rawArguments["--in"] + ".ann");
                }

                if (!rawArguments.ContainsKey("--layers"))
                {
                    rawArguments.Add("--layers", "0:0");
                }

                if (!rawArguments.ContainsKey("--threshold"))
                {
                    rawArguments.Add("--threshold", "0.25:0.75:0.25");
                }

                if (!rawArguments.ContainsKey("--iterations"))
                {
                    rawArguments.Add("--iterations", "100");
                }

                if (!rawArguments.ContainsKey("--mintrades"))
                {
                    rawArguments.Add("--mintrades", "10");
                }

                if (!rawArguments.ContainsKey("--randomize"))
                {
                    rawArguments.Add("--randomize", "0");
                }

                if (!rawArguments.ContainsKey("--validate"))
                {
                    rawArguments.Add("--validate", rawArguments["--in"]);
                }

                Console.WriteLine("Argumente:");

                foreach (KeyValuePair<string, string> entry in rawArguments)
                {
                    Console.WriteLine(entry.Key + ": " + entry.Value);
                }

                Console.WriteLine("");

                Optimizer.Optimize(rawArguments);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler: {0}\r\n", ex.Message);
                ShowHelp();
            }
            
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
        }

        static void ShowInitText()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.FullName.Split(',')[0];
            string version = assembly.FullName.Split(',')[1].Split('=')[1];

            Console.WriteLine("{0} {1}", name, version);
            Console.WriteLine("Copyright (c) 2015 Dennis Kempf");
            Console.WriteLine();
        }

        /// <summary>
        /// Zeigt die Programmhilfe an.
        /// </summary>
        static void ShowHelp()
        {
            Console.WriteLine("Argumente:");
            Console.WriteLine("\t--in: Eingabedatei mit Trainingsdaten im FSD-Format");
            Console.WriteLine("\t--out: Ausgabedatei für das optimierte Netzwerk");
            Console.WriteLine("\t--layers: Layerkonfiguration im Format A1:B1;A2:B2;...;An:Bn");
            Console.WriteLine("\t--threshold: Simulationsschwellenwert im Format MINIMUM:MAXIMUM:SCHRITTGRÖSSE");
            Console.WriteLine("\t--iterations: Maximale Anzahl an Lernzyklen pro Konfiguration.");
            Console.WriteLine("\t--mintrades: Minimale Anzahl an simulierten Trades für jeden Überprüfungsdatensatz.");
            Console.WriteLine("\t--randomize: Setzt das Netzwerk alle X Iterationen zurück");
            Console.WriteLine("\t--validate: Dateien (mit ; getrennt) im FSD-Format, für die das Netzwerk optimiert werden soll.");
        }
    }
}
