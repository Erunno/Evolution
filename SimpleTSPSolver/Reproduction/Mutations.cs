using System;
using System.Collections.Generic;
using System.Text;
using Evolution;

namespace SimpleTSPSolver
{
    class SwichingMutation : IMutation<Cycle>
    {
        public SwichingMutation(EnvironmentOf<Cycle> environment, int seed)
        {
            myEnvinronment = environment;
            rnd = new Random(seed);
        }

        EnvironmentOf<Cycle> myEnvinronment;

        Random rnd;

        public double MutationRate { get; set; }

        public Cycle GetMutatedCreature(Cycle parent)
        {
            int maxNumOfSwiches = (int)(parent.Verticies.Length * MutationRate * 3);

            int numOfSwiches;

            numOfSwiches = (int)(rnd.NextDouble() * maxNumOfSwiches) + 1; //at least one swich will be performed

            int[] newCycleArray = GetNewCycleArray(parent.Verticies.Length);
            for (int i = 0; i < newCycleArray.Length; i++)
                newCycleArray[i] = parent.Verticies[i];

            for (int i = 0; i < numOfSwiches; i++)
            {
                int k, j;
                k = rnd.Next(0, parent.Verticies.Length);
                j = rnd.Next(0, parent.Verticies.Length);

                newCycleArray.Swich(k, j);
            }

            return new Cycle(newCycleArray);
        }

        private int[] GetNewCycleArray(int size)
        {
            lock (myEnvinronment.DisposedCreatures)
                if (!myEnvinronment.DisposedCreatures.IsEmpty)
                    return myEnvinronment.DisposedCreatures.ExtractCreature().Verticies;

            return new int[size];
        }
    }

}
