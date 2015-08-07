using System;
using System.Globalization;
using System.IO;

namespace finances_optimize
{
    class FsdParser
    {
        public string FileContents { get; private set; }
        
        public double[][] InputVectors { get; private set; }
        public double[][] OutputVectors { get; private set; }

        public FsdParser(string fileContents)
        {
            FileContents = fileContents;

            generateInputVectors();
            generateOutputVectors();
        }

        public static FsdParser FromFile(string fileName)
        {
            return new FsdParser(File.ReadAllText(fileName));
        }

        private void generateInputVectors()
        {
            string[] samples = FileContents.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int numSamples = samples.Length;

            InputVectors = new double[numSamples][];

            for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
            {
                string[] sampleParts = samples[sampleIdx].Split(';');
                string[] inputValues = sampleParts[0].Split(',');
                int numInputValues = inputValues.Length;

                InputVectors[sampleIdx] = new double[numInputValues];

                for (int inputValueIdx = 0; inputValueIdx < numInputValues; inputValueIdx++)
                {
                    InputVectors[sampleIdx][inputValueIdx] = double.Parse(inputValues[inputValueIdx], CultureInfo.InvariantCulture);
                }
            }
        }

        private void generateOutputVectors()
        {
            string[] samples = FileContents.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int numSamples = samples.Length;

            OutputVectors = new double[numSamples][];

            for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
            {
                string[] sampleParts = samples[sampleIdx].Split(';');
                string[] outputValues = sampleParts[1].Split(',');

                double open = double.Parse(outputValues[0], CultureInfo.InvariantCulture);
                double close = double.Parse(outputValues[1], CultureInfo.InvariantCulture);

                OutputVectors[sampleIdx] = new double[]
                    { close > open ? 1 : -1, open > close ? 1 : -1 };
            }
        }
    }
}
