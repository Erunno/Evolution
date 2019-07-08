using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution
{
    /// <summary>
    /// Provides EnvinronmentOf<<typeparamref name="Creature"/>> with nessesary informations
    /// </summary>
    /// <typeparam name="Creature"></typeparam>
    class StartingInfo<Creature>
    {
        public StartingInfo(ISelector<Creature> selector, IFitnessFunctionFactory<Creature> fitnessFunctionFactory)
        {
            Selector = selector;
            FitnessFunctionFactory = fitnessFunctionFactory;
        }

        /// <param name="fitnessFunction">Have to thread save since it will be used in several thread at the same time</param>
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

        /// <summary>
        /// Is inicialized with number of logical processors
        /// </summary>
        public int NumberOfRunningThreads { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// Have to be in range [0,1]
        /// </summary>
        public double MutationRate { get; set; } = 0.05;

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
        public NewBestCretureFoundEventDelegate<Creature> newBestCreture;

        internal ISelector<Creature> GetSelector()
        {
            if (Selector != null) //selector has been provided by user
                return Selector;

            FitnessFunctionDelegate<Creature> tempFitnessFunction = FitnessFunctionFactory.CreateNewFitnessFunctionDelegate();
            RatedCreature<Creature> ratedForeFather = new RatedCreature<Creature>(foreFather, tempFitnessFunction(foreFather));

            return new DefaultSelector<Creature>(newBestCreture, ratedForeFather, NumberOfSurvivals);
        }

        internal IFitnessFunctionFactory<Creature> GetFitnessFunctionFactory() => FitnessFunctionFactory;
    }
}
