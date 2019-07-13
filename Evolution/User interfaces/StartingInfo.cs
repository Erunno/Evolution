using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution
{
    /// <summary>
    /// Provides EnvinronmentOf<<typeparamref name="Creature"/>> with nessesary informations
    /// </summary>
    /// <typeparam name="Creature"></typeparam>
    public class StartingInfo<Creature>
    {
        public StartingInfo(ISelector<Creature> selector, IFitnessFunctionFactory<Creature> fitnessFunctionFactory)
        {
            Selector = selector;
            FitnessFunctionFactory = fitnessFunctionFactory;
        }

        /// <param name="fitnessFunction">Have to thread save since it will be used in several threads at the same time</param>
        public StartingInfo(FitnessFunctionDelegate<Creature> fitnessFunction, Creature foreFather)
        {
            FitnessFunctionFactory = new DefaultFitnessFunctionFactory<Creature>(fitnessFunction);
            this.foreFather = foreFather;
        }

        public StartingInfo(FitnessFunctionProvider<Creature> fitnessFunctionProvider, Creature foreFather)
        {
            FitnessFunctionFactory = new DefaultFitnessFunctionFactory<Creature>(fitnessFunctionProvider);
            this.foreFather = foreFather;
        }

        private Creature foreFather;

        private ISelector<Creature> Selector { get; set; }
        private IFitnessFunctionFactory<Creature> FitnessFunctionFactory { get; set; }

        private int _numberOfRunningThreads = Environment.ProcessorCount;
        /// <summary>
        /// Is inicialized with number of logical processors
        /// </summary>
        public int NumberOfRunningThreads
        {
            get => _numberOfRunningThreads;
            set
            {
                if (value < 1)
                    throw new UnvalidValueException($"Value of {nameof(NumberOfRunningThreads)} have to possitive number.");

                _numberOfRunningThreads = value;
            }
        }

        private double _mutationRate = 0.05;
        /// <summary>
        /// Have to be in range [0,1]
        /// </summary>
        public double MutationRate
        {
            get => _mutationRate;
            set
            {
                if (value < 0 || 1 < value)
                    throw new UnvalidValueException($"Value of {nameof(NumberOfRunningThreads)} have to possitive number.");

                _mutationRate = value;
            }
        }

        /// <summary>
        /// Should be significantly bigger than Size of population 
        /// (at least 5x bigger)
        /// </summary>
        public int SizeOfPopulation { get; set; } = 100;

        /// <summary>
        /// Should be significantly smaller than Size of population 
        /// (at least 5x smaller)
        /// </summary>
        public int NumberOfSurvivals { get; set; } = 1000;

        /// <summary>
        /// Is useful only if default selector is used (i.e. was not called consturctor with ISelector<Creature> in argument)
        /// And even that it is optional
        /// </summary>
        public NewBestCretureFoundEventDelegate<Creature> NewBestCreture;

        internal ISelector<Creature> GetSelector(EnvironmentOf<Creature> environment)
        {
            if (Selector != null) //selector has been provided by user
                return Selector;

            FitnessFunctionDelegate<Creature> tempFitnessFunction = FitnessFunctionFactory.CreateNewFitnessFunctionDelegate();
            RatedCreature<Creature> ratedForeFather = new RatedCreature<Creature>(foreFather, tempFitnessFunction(foreFather));

            return new DefaultSelector<Creature>(NewBestCreture, ratedForeFather, NumberOfSurvivals, environment.DisposedCreatures);
        }

        internal IFitnessFunctionFactory<Creature> GetFitnessFunctionFactory() => FitnessFunctionFactory;
    }

    class UnvalidValueException : Exception
    {
        public UnvalidValueException() { }

        public UnvalidValueException(string message)
            : base(message)
        {
        }
    }
}
