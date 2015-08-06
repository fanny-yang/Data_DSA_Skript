using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deep_Learning_for_Binary_Options
{
    class StochasticDataParser : IDataParser
    {
        private const int SAMPLE_POS_DATETIME = 0;
        private const int SAMPLE_POS_OPEN = 1;
        private const int SAMPLE_POS_CLOSE = 2;
        private const int SAMPLE_POS_STOCHASTIC = 3;

        private const int HISTORY_LOOKUP = 8;

        private string[] fileContents;
        private double[][] samples;
        private double[][] labels;

        public StochasticDataParser(string fileName)
        {
            fileContents = File.ReadAllLines(fileName);
            makeSamples();
            makeLabels();
        }

        public double[][] GetLabels()
        {
            return labels;
        }

        public double[][] GetSamples()
        {
            return samples;
        }

        public int[] GetLayerSizes()
        {
            return new int[] { HISTORY_LOOKUP, 1 };
        }

        private void makeLabels()
        {
            labels = new double[fileContents.Length - HISTORY_LOOKUP][];
            
            for (int sIdx = HISTORY_LOOKUP; sIdx < samples.Length + HISTORY_LOOKUP; sIdx++)
            {
                labels[sIdx - HISTORY_LOOKUP] = new double[] { getNormalizedDirection(sIdx) };
            }
        }

        private void makeSamples()
        {
            samples = new double[fileContents.Length - HISTORY_LOOKUP][];

            for (int sIdx = HISTORY_LOOKUP; sIdx < samples.Length + HISTORY_LOOKUP; sIdx++) 
            {
                samples[sIdx - HISTORY_LOOKUP] = new double[HISTORY_LOOKUP];

                for (int i = 0; i < samples[sIdx - HISTORY_LOOKUP].Length; i++)
                {
                    samples[sIdx - HISTORY_LOOKUP][i] = getNormalizedStochastic(sIdx - i - 1);
                }
            }
        }

        private double getNormalizedStochastic(int idx)
        {
            string[] sampleContents = fileContents[idx].Split(',');

            double stochastic = double.Parse(sampleContents[SAMPLE_POS_STOCHASTIC], CultureInfo.InvariantCulture);
            double stochasticNormalized = stochastic / 100.0;

            return stochasticNormalized;
        }

        private double getNormalizedDirection(int idx)
        {
            string[] sampleContents = fileContents[idx].Split(',');

            double open = double.Parse(sampleContents[SAMPLE_POS_OPEN], CultureInfo.InvariantCulture);
            double close = double.Parse(sampleContents[SAMPLE_POS_CLOSE], CultureInfo.InvariantCulture);

            return close > open ? 1 : 0;
        }
    }
}
