﻿using Spriden_random_sample_validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

/* Bobby Chapa
 * 4/8/2023
 * Read Spriden text file in pipe delimited format
 * Gets the first field (Spriden ID)
 * Write to text file a set of randomely non-recurring Spriden ids for use as a sample
 */

namespace SpridenRandomSampleValidation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = "";// name of the file to be examined. Do not include the extension
            int setSize = 0; // sample size
            int confidenceLevel = 0; // if enter is pushed without entering any value 95 will be used
            string filePath = "";

            try
            {
                // get file name
                Console.WriteLine("Enter file name minus extension:");
                fileName = Console.ReadLine();
                filePath = @"Y:\Director\CSRS_Banner_Validation\" + fileName + ".dat";

                // get confidence level. 95 is default
                Console.WriteLine("Enter the Confidence Level 80, 85, 90, 95, 98, 99\r");
                if (int.TryParse(Console.ReadLine(), out confidenceLevel) && (confidenceLevel == 80 || confidenceLevel == 85 || confidenceLevel == 90 || confidenceLevel == 95 || confidenceLevel == 98 || confidenceLevel == 99))
                {
                    Console.WriteLine("\nYou entered: " + confidenceLevel);
                }
                else
                {
                    Console.WriteLine("Default value of 95 will be used.");
                    confidenceLevel = 95;
                }

                // read dat file and retrieve first field per line
                SpridenFileReader fileReader = new SpridenFileReader(filePath);
                List<string> spridenIdList = fileReader.ReadSpridenIds();

                // find population size
                int populationSize = spridenIdList.Count-1; // do not count first row. it is the column names

                // set population size and confidence level
                var sampleSizeCalculator = new Spriden_random_sample_validation.SampleSizeCalculator
                {
                    _confidenceLevel = confidenceLevel,
                    _populationSize = populationSize
                };

                // calculate sample size amd output user information to console
                Console.WriteLine("\nPopulation size: ");
                Console.WriteLine(populationSize);
                Console.WriteLine("\nSample size: ");
                setSize = sampleSizeCalculator.CalculateSampleSize(); // calc sample size
                Console.WriteLine(setSize);
                Console.ReadLine();

                // calculate the sample from the dat file
                RandomSampleGenerator generator = new RandomSampleGenerator(spridenIdList, setSize);
                List<string> randomSample = generator.GetRandomSample();

                // write sample output to text files
                SampleFileWriter sampleFileWriter = new SampleFileWriter(Path.GetDirectoryName(filePath), fileName);
                sampleFileWriter.WriteSampleToFile(randomSample);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"The file '{filePath}' was not found.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.ReadLine();
            }
        }
    }

    // reads the input file and extracts the SPRIDEN IDs
    public class SpridenFileReader
    {
        private readonly string _filePath;

        public SpridenFileReader(string filePath)
        {
            _filePath = filePath;
        }

        public List<string> ReadSpridenIds()
        {
            List<string> spridenIdList = new List<string>();
            string pattern = @"\|(.*?)\|";
            Regex regex = new Regex(pattern);

            using (StreamReader reader = new StreamReader(File.OpenRead(_filePath)))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    Match match = regex.Match(line);
                    if (match.Success)
                    {
                        string result = match.Groups[1].Value;
                        spridenIdList.Add(result);
                    }
                }
            }

            return spridenIdList;
        }
    }

    // generates a non recurring random sample from the list of SPRIDEN IDs 
    public class RandomSampleGenerator
    {
        private readonly List<string> _list;
        private readonly int _setSize;

        public RandomSampleGenerator(List<string> list, int setSize)
        {
            _list = list;
            _setSize = setSize;
        }

        public List<string> GetRandomSample()
        {
            int min = 0;
            int max = _list.Count;
            HashSet<int> uniqueIndexes = GenerateUniqueRandomNumbers(_setSize, min, max);

            List<string> randomSample = new List<string>();
            foreach (int index in uniqueIndexes)
            {
                randomSample.Add(_list[index]);
            }

            return randomSample;
        }

        private static HashSet<int> GenerateUniqueRandomNumbers(int setSize, int min, int max)
        {
            HashSet<int> uniqueNumbers = new HashSet<int>();
            Random random = new Random();

            while (uniqueNumbers.Count < setSize)
            {
                int randomNumber = random.Next(min, max);
                uniqueNumbers.Add(randomNumber);
            }

            return uniqueNumbers;
        }
    }

    // write the random sample to a text file
    public class SampleFileWriter
    {
        private readonly string _fileDirectory;
        private readonly string _fileName;

        public SampleFileWriter(string fileDirectory, string fileName)
        {
            _fileDirectory = fileDirectory;
            _fileName = fileName;
        }

        public void WriteSampleToFile(List<string> sampleData)
        {
            string subdir = _fileDirectory + @"\Random_Samples\";

            if (!Directory.Exists(subdir))
            {
                _ = Directory.CreateDirectory(subdir);
            }

            string outputPath = Path.Combine(subdir, _fileName + "_sample.txt");

            using (StreamWriter sw = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                foreach (string item in sampleData)
                {
                    sw.WriteLine(item);
                }
            }
        }
    }


}
