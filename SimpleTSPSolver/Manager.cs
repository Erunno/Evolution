using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Evolution;

namespace SimpleTSPSolver
{
    class Manager
    {
        public Manager(TextReader valuesTable)
        {
            fitnessFunction = TSPLoader.LoadFitnessFunctionFrom(valuesTable);
        }

        private TSPFitnessFunction fitnessFunction;

        public void RunEvolution()
        {
            EnvironmentOf<Cycle> environment = GetEnvironment();

            while (true)
                environment.RunEvolution(1000);

        }

        private EnvironmentOf<Cycle> GetEnvironment()
        {
            Cycle foreFather = new Cycle(fitnessFunction.VerticiesCount);

            for (int i = 0; i < foreFather.Verticies.Length; i++)
                foreFather.Verticies[i] = i;

            StartingInfo<Cycle> startingInfo = new StartingInfo<Cycle>(fitnessFunction.Evaluate, foreFather);
            startingInfo.NumberOfRunningThreads = Environment.ProcessorCount;
            startingInfo.MutationRate = 0.10;
            startingInfo.NewBestCreture = NewBestFound;

            startingInfo.SizeOfPopulation = 10_000;
            startingInfo.NumberOfSurvivals = 1_000;

            Random rnd = new Random(42); //todo remove seed

            EnvironmentOf<Cycle> environment = new EnvironmentOf<Cycle>(startingInfo);
            environment.AddMutationProvider(() => new SwichingMutation(environment, rnd.Next()));

            environment.DisposedCreatures.SetStoreCreatures();

            return environment;
        }

        object Lock = new object();
        private void NewBestFound(Cycle cycle, double value)
        {
            lock (Lock)
            {
                Console.WriteLine("New best cycle found:");
                Console.WriteLine($"Value: {value}");
                Console.WriteLine(cycle);
                Console.WriteLine();
            }
        }
    }
}
