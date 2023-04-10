using System;

namespace Spriden_random_sample_validation
{
    class SampleSizeCalculator
    {
        private int _confidenceLevel;

        public SampleSizeCalculator(int confidenceLevel)
        {
            _confidenceLevel = confidenceLevel;
        }

        public int CalculateSampleSize(int populationSize)
        {
            double zScore = 0.0;

            if (_confidenceLevel == 95)
                zScore = 1.96;
            else if (_confidenceLevel == 99)
                zScore = 2.58;
            else
            {
                Console.WriteLine("Invalid confidence level. Use 95 or 99.");
                return -1;
            }

            double estimatedProportion = 0.5;
            double marginOfError = 0.05;

            int sampleSize = ComputeSampleSize(populationSize, zScore, estimatedProportion, marginOfError);
            return sampleSize;
        }

        private int ComputeSampleSize(int populationSize, double zScore, double estimatedProportion, double marginOfError)
        {
            double numerator = Math.Pow(zScore, 2) * estimatedProportion * (1 - estimatedProportion) * populationSize;
            double denominator = (Math.Pow(marginOfError, 2) * (populationSize - 1)) + (Math.Pow(zScore, 2) * estimatedProportion * (1 - estimatedProportion));
            int sampleSize = (int)Math.Ceiling(numerator / denominator);

            return sampleSize;
        }
    }
}

