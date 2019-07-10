using System;
using System.IO;

namespace SimpleTSPSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader input = new StreamReader("input");

            Manager manger = new Manager(input);

            manger.RunEvolution();
        }
    }
}
