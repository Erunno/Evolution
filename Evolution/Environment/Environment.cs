﻿using System;
using System.Collections.Generic;

namespace Evolution
{
    class EnvironmentOf<Creature>
    {
        public EnvironmentOf()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Is used to select best of each generation
        /// It must not be emty - at least one Creture have to be present
        /// </summary>
        public ISelector<Creature> Selector { get; set; }

        Random rnd = new Random();

        List<IMutation<Creature>> mutations = new List<IMutation<Creature>>();
        List<ISexualReproduction<Creature>> sexualReproductions = new List<ISexualReproduction<Creature>>();
        List<IAsexualReproduction<Creature>> asexualReproductions = new List<IAsexualReproduction<Creature>>();

        /// <summary>
        /// Maximal number of individuals in one step
        /// </summary>
        public int SizeOfPupulation { get; set; } = 1000;

        private int _numOfSurvivals = 100;
        /// <summary>
        /// Maximal number of survivals from generation n to generation n+1
        /// 
        /// Setting new value can mean memory allocation (in case of default selector)
        /// therefore it is not good idea doing it often
        /// </summary>
        public int NumberOfSurvivals {
            get => _numOfSurvivals;
            private set
            {
                if (Selector.GetType() == typeof(DefaultSelector<Creature>))
                    ((DefaultSelector<Creature>)Selector).ChangeSizeOfHeap(value);

                _numOfSurvivals = value;
            }
        }

        /// <summary>
        /// It is initialized with the number of logical cores
        /// 
        /// Means number of thread computing evolution i.e. thread managing other threads can be running
        /// </summary>
        public int MaximalNumOfRunningThreads { get; set; } = System.Environment.ProcessorCount;

        public int NumOfGenerationSoFar { get; private set; }

        /// <summary>
        /// Determines how aggressive mutations are. Can be changed automatically to reach the best result possible.
        /// It is in range [0,1]
        /// </summary>
        public double MutationRate { get; set; }

        /// <summary>
        /// Creates and sets new default selector using fitness function in argument.
        /// First Creature is needed to start evolution
        /// </summary>
        public void CreateAndSetDefaultSelector(FitnessFunctionDelegate<Creature> fitnessFunction, Creature foreFather)
        {
            Selector = new DefaultSelector<Creature>(fitnessFunction, NumberOfSurvivals);
        }

        /// <summary>
        /// Is called when new best creature is found
        /// </summary>
        public NewBestCretureFoundEventDelegate<Creature> NewBestFoundEvent { get; set; } = (x) => { };

        /// <summary>
        /// Adds new way of asexual reproduction
        /// </summary>
        public void AddAsexualReproduction(IAsexualReproduction<Creature> asexualReproduction)
        {
            asexualReproductions.Add(asexualReproduction);
        }

        /// <summary>
        /// Adds new way of sexual reproduction
        /// </summary>
        public void AddSexualReproduction(ISexualReproduction<Creature> sexualReproduction)
        {
            sexualReproductions.Add(sexualReproduction);
        }

        /// <summary>
        /// Adds new way of mutation
        /// </summary>
        public void AddMutation(IMutation<Creature> mutation)
        {
            mutations.Add(mutation);
        }

        /// <summary>
        /// Returns set of best creatures based on their rating. (Performs sorting)
        /// </summary>
        /// <param name="count">Size of returned IEnumerable</param>
        public IEnumerable<RatedCreature<Creature>> GetBestRatedCreatures(int count)
        {
            return Selector.GetBestCreatures(count);
        }

        /// <summary>
        /// Runs evolution 
        /// Before calling this method it is nessesary that following components are present:
        ///     Mandatory (throws exeption):
        ///     Selector (or at least fitness function passed to "CreateAndSetDefaultSelector")
        ///     
        ///     Optional (but good idea to set):
        ///     NewBestFoundEvent
        ///     SizeOfPupulation
        ///     NumberOfSurvivals
        ///     MaximalNumOfRunningThreads
        /// </summary>
        public void RunEvolution(int numOfSteps)
        {
            throw new NotImplementedException();
        }

        internal IRandomReproductionPicker<Creature> GetRandomReproductionPicker()
            => new RandomReprodictionPicker<Creature>(this, rnd.Next());

        private interface IRndRepPickerGetter<T>
        {
            RandomReprodictionPicker<T> GetRandomReprodictionPicker(EnvironmentOf<T> environment, int rndSeed);
        }

        private class RandomReprodictionPicker<CreatureOfRepPicker> : IRandomReproductionPicker<CreatureOfRepPicker>
        {
            EnvironmentOf<CreatureOfRepPicker> environment;
            Random rnd;

            public RandomReprodictionPicker(EnvironmentOf<CreatureOfRepPicker> environment, int seed)
            {
                this.environment = environment;
                rnd = new Random(seed);
            }

            public IMutation<CreatureOfRepPicker> GetRandomMutation()
            {
                throw new NotImplementedException();
            }

            public ISexualReproduction<CreatureOfRepPicker> GetRandomSexualReproduction()
            {
                throw new NotImplementedException();
            }

            public IAsexualReproduction<CreatureOfRepPicker> GetRandomAsexualReproduction()
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// It will randomly choose wanted mean of reproduction
    /// </summary>
    interface IRandomReproductionPicker<Creature>
    {
        IAsexualReproduction<Creature> GetRandomAsexualReproduction();
        ISexualReproduction<Creature> GetRandomSexualReproduction();
        IMutation<Creature> GetRandomMutation();
    }
}
