using Accord.Neuro.Learning;
using AForge.Neuro;
using AForge.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deep_Learning_for_Binary_Options
{
    class Learner
    {
        private const string PARSER_STOCHASTIC = "stochastic";
        private const int DISPLAY_UPDATE_DELAY = 100;

        public static void Learn(string parserName, string fileToParse)
        {
            IDataParser dataParser = GetParser(parserName, fileToParse);
            double[][] inputs = dataParser.GetSamples();
            double[][] outputs = dataParser.GetLabels();

            IDataParser validationDataParser = null;
            double[][] validationInputs = null;
            double[][] validationOutputs = null;

            if (File.Exists("validation.csv"))
            {
                validationDataParser = GetParser(parserName, "validation.csv");
                validationInputs = validationDataParser.GetSamples();
                validationOutputs = validationDataParser.GetLabels();
            }

            if (false)
            {
                string logstr = "";

                for (int i = 0; i < inputs.Length; i++)
                {
                    logstr += "{ ";

                    for (int x = 0; x < inputs[i].Length; x++)
                    {
                        logstr += inputs[i][x].ToString();
                        if (x < inputs[i].Length - 1) logstr += ", ";
                    }

                    logstr += " }";
                    logstr += " => " + outputs[i][0] + "\r\n";
                }

                File.WriteAllText("log.txt", logstr);
            }

            ActivationNetwork network = null;
            string neuralNetworkSaveFileName = GetNeuralNetworkSaveFileName(fileToParse);

            if (File.Exists(neuralNetworkSaveFileName))
            {
                network = (ActivationNetwork)Network.Load(neuralNetworkSaveFileName);
                Console.WriteLine("Bestehendes neuronales Netzwerk geladen!");
            }
            else
            {
                network = new ActivationNetwork(
                   new SigmoidFunction(),
                   inputs[0].Length, dataParser.GetLayerSizes());
                Console.WriteLine("Neues neuronales Netzwerk erzeugt!");

                network.Randomize();
            }

            Console.WriteLine("");
            Console.Write("Layers:");
            foreach (Layer layer in network.Layers)
            {
                Console.WriteLine("\t{0,4} -> {1,-4}", layer.InputsCount, layer.Neurons.Length);
            }

            ParallelResilientBackpropagationLearning teacher = new ParallelResilientBackpropagationLearning(network);

            Console.WriteLine("");
            Console.WriteLine("[p] Lernprozess pausieren / fortsetzen");
            Console.WriteLine("[s] Stoppen und Änderungen speichern");
            Console.WriteLine("[c] Stoppen und Änderungen verwerfen");
            Console.WriteLine("[x] Stoppen und gespeichertes Netzwerk verwerfen");
            Console.WriteLine("");

            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;

            double lastError = double.PositiveInfinity;
            long lastStatsUpdate = Environment.TickCount;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    switch (Console.ReadKey().KeyChar)
                    {
                        case 'p':
                            Program.Pause();
                            break;

                        case 's':
                            network.Save(neuralNetworkSaveFileName);
                            return;

                        case 'c':
                            return;

                        case 'x':
                            if (File.Exists(neuralNetworkSaveFileName))
                            {
                                File.Delete(neuralNetworkSaveFileName);
                                return;
                            }
                            else
                            {
                                continue;
                            }
                            
                        
                        default:
                            continue;
                    }
                }

                if (Environment.TickCount - lastStatsUpdate > DISPLAY_UPDATE_DELAY)
                {
                    lastStatsUpdate = Environment.TickCount;

                    Console.SetCursorPosition(cursorLeft, cursorTop);
                    Console.Write(new string(' ', Console.BufferWidth * 4));
                    Console.SetCursorPosition(cursorLeft, cursorTop);

                    Console.Write("Learning...\nNetwork error: {0:0.00}\nWin-rate (training): {1:0.00}%\nWin-rate (validation): {2:0.00}%",
                        lastError, GetRealWinRate(network, inputs, outputs) * 100, validationDataParser != null ? GetRealWinRate(network, validationInputs, validationOutputs) * 100 : double.NaN);
                    lastError = teacher.RunEpoch(inputs, outputs);
                }
            }
        }

        private static IDataParser GetParser(string parserName, string fileToParse)
        {
            switch (parserName)
            {
                case PARSER_STOCHASTIC:
                    return new StochasticDataParser(fileToParse);

                default:
                    return null;
            }
        }

        private static string GetNeuralNetworkSaveFileName(string baseFileName)
        {
            return baseFileName + ".ann";
        }

        private static double GetRealWinRate(Network network, double[][] inputs, double[][] outputs)
        {
            int won = 0, lost = 0;

            for (int i = 0; i < inputs.Length; i++)
            {
                int networkOutput = network.Compute(inputs[i])[0] > 0.5 ? 1 : 0;
                int expectedOutput = outputs[i][0] > 0.5 ? 1 : 0;

                if (networkOutput == expectedOutput)
                {
                    won++;
                }
                else
                {
                    lost++;
                }
            }

            return (double)won / (won + lost);
        }
    }
}
