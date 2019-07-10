using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTSPSolver
{
    class TSPFitnessFunction
    {
        public TSPFitnessFunction(int[,] valuesOfEdges)
        {
            if (valuesOfEdges.GetLength(0) != valuesOfEdges.GetLength(1))
                throw new WrongDimensionsException();

            values = valuesOfEdges;
            VerticiesCount = valuesOfEdges.GetLength(0);
        }

        public int VerticiesCount { get; }
        int[,] values;

        public double Evaluate(Cycle cycle)
        {
            long sum = 0;

            for (int i = 0; i < cycle.Verticies.Length - 1; i++)
            {
                int vert1 = cycle.Verticies[i];
                int vert2 = cycle.Verticies[i + 1];

                sum += values[vert1, vert2];
            }

            int lastVert = cycle.Verticies[cycle.Verticies.Length - 1];
            int firstVert = cycle.Verticies[0];

            sum += values[lastVert, firstVert];

            return sum;
        }
    }
    

    class WrongDimensionsException : Exception { }
}
