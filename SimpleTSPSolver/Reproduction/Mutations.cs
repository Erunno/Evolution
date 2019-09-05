using System;
using System.Collections.Generic;
using System.Text;
using Evolution;

namespace SimpleTSPSolver
{
    abstract class CycleMutation
    {
        protected EnvironmentOf<Cycle> myEnvironment;
        protected int[] GetNewCycleArray(int size)
        {
            lock (myEnvironment.DisposedCreatures)
                if (!myEnvironment.DisposedCreatures.IsEmpty)
                    return myEnvironment.DisposedCreatures.ExtractCreature().Verticies;

            return new int[size];
        }
    }


    class SwichingMutation : CycleMutation, IMutation<Cycle>
    {
        public SwichingMutation(EnvironmentOf<Cycle> environment, int seed)
        {
            myEnvironment = environment;
            rnd = new Random(seed);
        }

        Random rnd;

        public double MutationRate { get; set; }

        public Cycle GetMutatedCreature(Cycle parent)
        {
            int maxNumOfSwiches = (int)(parent.Verticies.Length * MutationRate * 3);

            int numOfSwiches;

            numOfSwiches = (int)(rnd.NextDouble() * maxNumOfSwiches) + 1; //at least one swich will be performed

            int[] newCycleArray = GetNewCycleArray(parent.Verticies.Length);

            parent.Verticies.CopyTo(newCycleArray);

            for (int i = 0; i < numOfSwiches; i++)
            {
                int k, j;
                k = rnd.Next(0, parent.Verticies.Length);
                j = rnd.Next(0, parent.Verticies.Length);

                newCycleArray.Swich(k, j);
            }

            return new Cycle(newCycleArray);
        }
    }

    class SwichStrings : CycleMutation, IMutation<Cycle>
    {
        public double MutationRate { get; set; }

        public SwichStrings(EnvironmentOf<Cycle> environment, int rndSeed)
        {
            myEnvironment = environment;
            rnd = new Random(rndSeed);
        }

        private Random rnd { get; }

        public Cycle GetMutatedCreature(Cycle parent)
        {
            int sizeOfChunks = GetSizeOfSwichedChunks(parent);
            Tuple<int, int> indexes = GetStartIndexes(sizeOfChunks, parent); //they can overlap it doesnt matter actually so why to slow down computation

            int[] newCycle = GetNewCycleArray(parent.Verticies.Length);
            parent.Verticies.CopyTo(newCycle);

            for (int i = 0; i < sizeOfChunks; i++)
                newCycle.Swich(
                    (indexes.Item1 + i) % parent.Verticies.Length, 
                    (indexes.Item2 + i) % parent.Verticies.Length
                );

            return new Cycle(newCycle);
        }

        private int GetSizeOfSwichedChunks(Cycle parent)
            => rnd.Next(2, (int)Math.Max(2, parent.Verticies.Length * MutationRate));

        private Tuple<int,int> GetStartIndexes(int chunksSize, Cycle parent)
        {

            int i1 = rnd.Next(0, parent.Verticies.Length);
            int i2 = rnd.Next(0, parent.Verticies.Length);

            return new Tuple<int, int>(i1, i2);
        }
    }

}
