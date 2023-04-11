using System;

/*
 * Bobby Chapa
 * 4/10/2023
 * Calculates sample size based on the formula for finite populations
 */

namespace Spriden_random_sample_validation
{
    class SampleSizeCalculator
    {
        public int _confidenceLevel { get; set; }
        public int _populationSize { get; set; }

        public int CalculateSampleSize()
        { 
            double zScore = 0.0;

            if (_confidenceLevel == 95)
                zScore = 1.96;
            else if (_confidenceLevel == 99)
                zScore = 2.58;
            else if (_confidenceLevel == 98)
                zScore = 2.576;
            else if (_confidenceLevel == 90)
                zScore = 1.465;
            else if (_confidenceLevel == 85)
                zScore = 1.44;
            else if (_confidenceLevel == 80)
                zScore = 1.282;
            else if (_confidenceLevel == 0) // if 0 set to 95
                zScore = 1.96;
            else
            {
                Console.WriteLine("Invalid confidence level. Use 80,85,90,95,98 or 99.");

                return -1;
            }

            double estimatedProportion = 0.5;
            double marginOfError = 0.05;

            int sampleSize = ComputeSampleSize(_populationSize, zScore, estimatedProportion, marginOfError);

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

