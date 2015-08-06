using Accord.Neuro.Learning;
using AForge.Neuro;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace finances_optimize
{
    class Optimizer
    {
        public static void Optimize(Dictionary<string, string> rawArguments)
        {
            string inputFileName = rawArguments["--in"];
            string outputFileName = rawArguments["--out"];

            string[] layersStrs = rawArguments["--layers"].Split(';');
            int[][] layers = new int[layersStrs.Length][];
            for (int layerIdx = 0; layerIdx < layers.Length; layerIdx++)
            {
                string[] layerBounds = layersStrs[layerIdx].Split(':');

                int minimum = int.Parse(layerBounds[0]);
                int maximum = int.Parse(layerBounds[1]);

                layers[layerIdx] = new int[] { minimum, maximum };
            }

            string[] thresholdStrs = rawArguments["--threshold"].Split(':');
            double minThreshold = double.Parse(thresholdStrs[0], CultureInfo.InvariantCulture);
            double maxThreshold = double.Parse(thresholdStrs[1], CultureInfo.InvariantCulture);
            double thresholdStepSize = double.Parse(thresholdStrs[2], CultureInfo.InvariantCulture);

            int minTrades = int.Parse(rawArguments["--mintrades"]);
            int iterations = int.Parse(rawArguments["--iterations"]);
            int randomize = int.Parse(rawArguments["--randomize"]);
            string[] validationFileNames = rawArguments["--validate"].Split(';');

            Optimize(
                inputFileName, outputFileName,
                layers,
                minThreshold, maxThreshold, thresholdStepSize,
                minTrades,
                iterations,
                randomize,
                validationFileNames);
        }

        public static void Optimize(
            string inputFileName, string outputFileName,
            int[][] layers,
            double minThreshold, double maxThreshold, double thresholdStepSize,
            int minTrades,
            int iterations,
            int randomize,
            string[] validationFileNames)
        {
            FsdParser inputParser = FsdParser.FromFile(inputFileName);
            FsdParser[] validationParsers = new FsdParser[validationFileNames.Length];

            for (int valParIdx = 0; valParIdx < validationParsers.Length; valParIdx++)
            {
                validationParsers[valParIdx] = 
                    FsdParser.FromFile(validationFileNames[valParIdx]);
            }

            Optimize(
                inputParser, outputFileName,
                layers,
                minThreshold, maxThreshold, thresholdStepSize,
                minTrades,
                iterations,
                randomize,
                validationParsers);
        }

        public static void Optimize(
            FsdParser inputParser, string outputFileName,
            int[][] layers,
            double minThreshold, double maxThreshold, double thresholdStepSize,
            int minTrades,
            int iterations,
            int randomize,
            FsdParser[] validationParsers)
        {
            HashSet<int[]> layerCombinations;
            getLayerCombinations(layers, out layerCombinations);

            Network bestNetwork = null;
            double bestThreshold = double.NaN;
            double bestMinWinRate = double.NegativeInfinity;

            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;

            int numTotalIterations = (int)(layerCombinations.Count * ((maxThreshold - minThreshold) / thresholdStepSize + 1) * iterations);
            int numIterations = 0;

            foreach (int[] layerCombination in layerCombinations)
            {
                ActivationNetwork network = new ActivationNetwork(
                    new SigmoidFunction(),
                    inputParser.InputVectors[0].Length,
                    layerCombination);

                network.Randomize();

                ParallelResilientBackpropagationLearning teacher = new ParallelResilientBackpropagationLearning(network);

                for (double threshold = minThreshold; threshold <= maxThreshold; threshold += thresholdStepSize)
                {
                    for (int iteration = 1; iteration <= iterations; iteration++)
                    {
                        numIterations++;

                        Console.CursorLeft = cursorLeft;
                        Console.CursorTop = cursorTop;

                        Console.Write(new string(' ', Console.BufferWidth * 10));

                        Console.CursorLeft = cursorLeft;
                        Console.CursorTop = cursorTop;

                        Console.Write("layerCombination[]: { ");

                        for (int layerComponentIdx = 0; layerComponentIdx < layerCombination.Length; layerComponentIdx++)
                        {
                            Console.Write(layerCombination[layerComponentIdx]);
                            if (layerComponentIdx < layerCombination.Length - 1)
                                Console.Write(", ");
                        }

                        Console.WriteLine(" }");

                        Console.WriteLine("threshold: {0:0.00}", threshold);
                        Console.WriteLine("iteration: {0}", iteration);
                        Console.WriteLine("bestMinWinRate: {0:0.00}%", bestMinWinRate);
                        Console.WriteLine("");
                        Console.WriteLine("Progress: {0:0.00}%", (double)numIterations / numTotalIterations * 100.0);

                        if (randomize > 0 && (iteration - 1) % randomize == 0)
                            network.Randomize();

                        teacher.RunEpoch(inputParser.InputVectors, inputParser.OutputVectors);

                        bool validData = true;
                        double minWinRate = double.PositiveInfinity;

                        foreach (FsdParser validationParser in validationParsers)
                        {
                            int numTradesWon, numTradesLost;
                            double tradeWinRate;

                            getStatistics(network, validationParser, threshold, out numTradesWon, out numTradesLost, out tradeWinRate);

                            if (numTradesWon + numTradesLost < minTrades)
                            {
                                validData = false;
                                break;
                            }

                            minWinRate = Math.Min(minWinRate, tradeWinRate);

                            if (minWinRate < bestMinWinRate)
                            {
                                validData = false;
                                break;
                            }
                        }

                        if (validData)
                        {
                            bestNetwork = network;
                            bestThreshold = threshold;
                            bestMinWinRate = minWinRate;

                            network.Save(outputFileName);

                            // Konfigurationsinformationen speichern
                            string configuration = "";
                            configuration += "layerCombination[]: { ";

                            for (int layerComponentIdx = 0; layerComponentIdx < layerCombination.Length; layerComponentIdx++)
                            {
                                configuration += layerCombination[layerComponentIdx];
                                if (layerComponentIdx < layerCombination.Length - 1)
                                    configuration += ", ";
                            }

                            configuration += " }\r\n";

                            configuration += string.Format("threshold: {0:0.00}\r\n", threshold);
                            configuration += string.Format("iteration: {0}\r\n", iteration);
                            configuration += string.Format("bestMinWinRate: {0:0.00}%\r\n", bestMinWinRate);

                            File.WriteAllText(outputFileName + ".txt", configuration);
                        }
                    }
                }
            }
        }

        private static void getLayerCombinations(int[][] layer, out HashSet<int[]> combinations)
        {
            combinations = new HashSet<int[]>();
            getLayerCombinations(layer, ref combinations, 0, new List<int>());
        }

        private static void getLayerCombinations(
            int[][] layer, ref HashSet<int[]> combinations,
            int currentDepth, List<int> currentLayerComponents)
        {
            if (currentDepth == layer.Length)
            {
                List<int> layerConfig = new List<int>();

                foreach (int layerComponent in currentLayerComponents)
                {
                    if (layerComponent > 0) layerConfig.Add(layerComponent);
                }

                layerConfig.Add(2); // Ausgänge müssen hinzugefügt werden

                combinations.Add(layerConfig.ToArray());
            }
            else
            {
                for (int a = layer[currentDepth][0]; a <= layer[currentDepth][1]; a++)
                {
                    List<int> clonedList = new List<int>();
                    foreach (int item in currentLayerComponents)
                        clonedList.Add(item);

                    clonedList.Add(a);

                    getLayerCombinations(layer, ref combinations, currentDepth + 1, clonedList);
                }
            }
        }

        private static void getStatistics(Network network, FsdParser parser, double threshold,
            out int numTradesWon, out int numTradesLost, out double tradeWinRate)
        {
            numTradesWon = 0;
            numTradesLost = 0;

            int numSamples = parser.InputVectors.Length;

            for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
            {
                double[] computed = network.Compute(parser.InputVectors[sampleIdx]);

                if (computed[0] > threshold && computed[1] < threshold)
                {
                    // Netzwerk hat "Rise" errechnet
                    if (parser.OutputVectors[sampleIdx][0] == 1)
                    {
                        numTradesWon++;
                    }
                    else
                    {
                        numTradesLost++;
                    }
                }
                else if (computed[1] > threshold && computed[0] < threshold)
                {
                    // Netzwerk hat "Fall" errechnet
                    if (parser.OutputVectors[sampleIdx][1] == 1)
                    {
                        numTradesWon++;
                    }
                    else
                    {
                        numTradesLost++;
                    }
                }
            }

            tradeWinRate = (double)numTradesWon / (numTradesWon + numTradesLost);
        }
    }
}
