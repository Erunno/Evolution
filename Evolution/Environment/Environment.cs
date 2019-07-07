using System;
using System.Collections.Generic;

namespace Evolution
{
    class EnvironmentOf<Creature>
    {
        public EnvironmentOf()
        {
            interval = new IntervalOfChildrenCount<Creature>(this);
            MaximalNumOfRunningThreads = Environment.ProcessorCount;

            throw new NotImplementedException();
        }

        public EnvironmentOf(int numOfThreads)
        {
            interval = new IntervalOfChildrenCount<Creature>(this);
            MaximalNumOfRunningThreads = numOfThreads > 0 ? numOfThreads : throw new InvalidOperationException();

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

        private int _sizeOfPopulation = 1000;
        /// <summary>
        /// Maximal number of individuals in one step
        /// 
        /// It should be significantly higher than NumberOfSurvivals (at least 5 times bigger)
        /// </summary>
        public int SizeOfPupulation
        {
            get => _sizeOfPopulation;
            set
            {
                _sizeOfPopulation = value;
                interval.RecomputeInterval();
            }
        }

        private int _numOfSurvivals = 100;
        /// <summary>
        /// Maximal number of survivals from generation n to generation n+1
        /// 
        /// Setting new value can mean memory allocation (in case of default selector)
        /// therefore it is not good idea doing it often
        /// </summary>
        public int NumberOfSurvivals {
            get => _numOfSurvivals;
            set
            {
                if (Selector.GetType() == typeof(DefaultSelector<Creature>))
                    ((DefaultSelector<Creature>)Selector).ChangeSizeOfHeap(value);

                _numOfSurvivals = value;
                interval.RecomputeInterval();
            }
        }

        private IntervalOfChildrenCount<Creature> interval;

        /// <summary>
        /// Means number of thread computing evolution i.e. thread managing other threads can be running
        /// 
        /// If not given, it will be number of logical cores
        /// </summary>
        public int MaximalNumOfRunningThreads { get; }

        public int NumOfGenerationSoFar { get; private set; }

        private double _mutationRate = 0.05;
        /// <summary>
        /// Determines how aggressive mutations are. Can be changed automatically to reach the best result possible.
        /// It is in range [0,1]
        /// </summary>
        public double MutationRate {
            get => _mutationRate;
            set
            {
                if (value < 0 || 1 > value)
                    throw new UnvalidMutationRateException();

                foreach (var mutation in mutations)
                    mutation.MutationRate = value;

                _mutationRate = value;
            }
        }

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
            mutation.MutationRate = MutationRate;
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
                int randomIndex = rnd.Next(0, environment.mutations.Count);
                return environment.mutations[randomIndex];
            }

            public ISexualReproduction<CreatureOfRepPicker> GetRandomSexualReproduction()
            {
                int randomIndex = rnd.Next(0, environment.sexualReproductions.Count);
                return environment.sexualReproductions[randomIndex];
            }

            public IAsexualReproduction<CreatureOfRepPicker> GetRandomAsexualReproduction()
            {
                int randomIndex = rnd.Next(0, environment.asexualReproductions.Count);
                return environment.asexualReproductions[randomIndex];
            }

            public int GetNumOfChildren() => rnd.Next(environment.interval.From, environment.interval.To);

            public MeanOfReproduction GetRandomMeanOfReproduction() => (MeanOfReproduction)rnd.Next(0, 3);
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
        int GetNumOfChildren();
        MeanOfReproduction GetRandomMeanOfReproduction();
    }

    enum MeanOfReproduction { sexual, asexual, mutation }

    class IntervalOfChildrenCount<Creature>
    {
        public int From, To;
        private EnvironmentOf<Creature> environment;

        public IntervalOfChildrenCount(EnvironmentOf<Creature> environment)
        {
            this.environment = environment;
            RecomputeInterval();
        }

        private const int Variation = 3;

        public void RecomputeInterval()
        {
            int meanValueOfChildren = (environment.SizeOfPupulation + environment.NumberOfSurvivals) / environment.NumberOfSurvivals; //rounded up

            From = meanValueOfChildren - Variation;
            To = meanValueOfChildren + Variation + 1; //upper bound is exclusive
        }
    }

    class UnvalidMutationRateException : Exception { }
}
