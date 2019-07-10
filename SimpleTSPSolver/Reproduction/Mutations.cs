using System;
using System.Collections.Generic;
using System.Text;
using Evolution;

namespace SimpleTSPSolver
{
    class SwichingMutation : IMutation<Cycle>
    {
        public SwichingMutation(EnvironmentOf<Cycle> environment)
        {
            myEnvinronment = environment;
        }

        EnvironmentOf<Cycle> myEnvinronment;

        Random rnd = new Random(42); //todo remove seed

        public double MutationRate { get; set; }

        public Cycle GetMutatedCreature(Cycle parent)
        {
            int maxNumOfSwiches = (int)(parent.Verticies.Length * MutationRate * 3);

            int numOfSwiches;

            lock (rnd)
                numOfSwiches = (int)(rnd.NextDouble() * maxNumOfSwiches) + 1; //at least one swich will be performed

            int[] newCycleArray = new int[parent.Verticies.Length];
            for (int i = 0; i < newCycleArray.Length; i++)
                newCycleArray[i] = parent.Verticies[i];

            for (int i = 0; i < numOfSwiches; i++)
            {
                int k, j;
                lock (rnd)
                {
                    k = rnd.Next(0, parent.Verticies.Length);
                    j = rnd.Next(0, parent.Verticies.Length);
                }

                newCycleArray.Swich(k, j);
            }

            return new Cycle(newCycleArray);
        }
    }

}
