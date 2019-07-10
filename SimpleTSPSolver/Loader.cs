using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SimpleTSPSolver
{
    class TSPLoader
    {
        public static TSPFitnessFunction LoadFitnessFunctionFrom(TextReader input)
        {
            if (!int.TryParse(input.ReadLine(), out int length))
                throw new FormatException("First line: there is no number");

            int[,] values = new int[length,length];

            for (int i = 0; i < length; i++)
            {
                string line = input.ReadLine();

                string[] numbers = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (numbers.Length != length)
                    throw new FormatException($"Table line {i}: Wrong number of fields");

                for (int j = 0; j < length; j++)
                {
                    if (!int.TryParse(numbers[j], out int value_ij))
                        throw new FormatException($"Line {i}, Column {j} of table: Field is not a number");

                    values[i, j] = value_ij;
                }
            }

            return new TSPFitnessFunction(values);
        }
    }
}
