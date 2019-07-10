using System;
using System.Collections.Generic;

namespace Evolution
{
    public class EnvironmentOf<Creature>
    {
        /// <summary>
        /// Nuber of threads is defautly set to number of logical cores
        /// </summary>
        public EnvironmentOf(StartingInfo<Creature> startingInfo)
        {
            Selector = startingInfo.GetSelector(this);
            FitnessFunctionFactory = startingInfo.GetFitnessFunctionFactory();

            interval = new IntervalOfChildrenCount<Creature>(this);

            NumberOfSurvivals = startingInfo.NumberOfSurvivals;
            SizeOfPupulation = startingInfo.SizeOfPopulation;
            MutationRate = startingInfo.MutationRate;
            MaximalNumOfRunningThreads = startingInfo.NumberOfRunningThreads;

            CreateOrUpdateComputationManager();
        }

        /// <summary>
        /// Is used to select best of each generation
        /// It must not be emty - at least one Creture have to be present
        /// </summary>
        public ISelector<Creature> Selector { get; set; }

        /// <summary>
        /// Store of creatures which are not good enought
        /// User can extract creatures and reuse them
        /// 
        /// It doesnt have to be implemented 
        /// but it deacreases pressure on garbage collector
        /// </summary>
        public DisposedCreaturesStore<Creature> DisposedCreatures { get; } = new DisposedCreaturesStore<Creature>();

        /// <summary>
        /// Creates new fitness function.
        /// 
        /// (Every thread gets one fitness function that means that fitness function doesnt have to be static)
        /// </summary>
        public IFitnessFunctionFactory<Creature> FitnessFunctionFactory { get; private set; } 

        public void SetFitnessFunctinonFactory(IFitnessFunctionFactory<Creature> newFitnessFunctionFactory)
        {
            FitnessFunctionFactory = newFitnessFunctionFactory;
            CreateOrUpdateComputationManager();
        }

        private void CreateOrUpdateComputationManager()
        {
            if (computationManager == null)
                computationManager = new ComputationManager<Creature>(this);

            computationManager.UpdateFitnessFunction();
        }

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

        /// <summary>
        /// Maximal number of survivals from generation n to generation n+1
        /// </summary>
        public int NumberOfSurvivals { get; private set; } = 100;

        /// <summary>
        /// Setting new value can mean memory allocation (in case of default selector)
        /// therefore it is not good idea doing it often
        /// </summary>
        /// <param name="newNumberOfSurvivals"></param>
        public void SetNumberOfSurvivals(int newNumberOfSurvivals)
        {
            if (Selector.GetType() == typeof(DefaultSelector<Creature>))
                ((DefaultSelector<Creature>)Selector).ChangeSizeOfHeap(newNumberOfSurvivals);

            NumberOfSurvivals = newNumberOfSurvivals;
            interval.RecomputeInterval();
        }

        private IntervalOfChildrenCount<Creature> interval;

        /// <summary>
        /// Means number of thread computing evolution i.e. thread managing other threads can be running
        /// 
        /// If not given, it will be number of logical cores
        /// </summary>
        public int MaximalNumOfRunningThreads { get; } //todo user can change it

        public int NumOfGenerationSoFar { get; private set; } = 0;

        /// <summary>
        /// Determines how aggressive mutations are. Can be changed automatically to reach the best result possible.
        /// It is in range [0,1]
        /// </summary>
        public double MutationRate { get; private set; } = 0.05;

        /// <summary>
        /// Sets new rate to all of mutations which were provided
        /// </summary>
        /// <param name="newRate">Have to be in interval [0,1] (inclusive)</param>
        public void SetMutationRate(int newRate)
        {
            if (newRate < 0 || 1 > newRate)
                throw new UnvalidMutationRateException();

            foreach (var mutation in mutations)
                mutation.MutationRate = newRate;

            MutationRate = newRate;
        }

        //todo change interface and add providers

        /// <summary>
        /// Adds new way of asexual reproduction
        /// </summary>
        public void AddAsexualReproduction(IAsexualReproduction<Creature> asexualReproduction)
        {
            asexualReproductions.Add(asexualReproduction);

            implementedMeansOfReproduction.Add(MeanOfReproduction.asexual);
        }

        /// <summary>
        /// Adds new way of sexual reproduction
        /// </summary>
        public void AddSexualReproduction(ISexualReproduction<Creature> sexualReproduction)
        {
            sexualReproductions.Add(sexualReproduction);

            implementedMeansOfReproduction.Add(MeanOfReproduction.sexual);
        }

        /// <summary>
        /// Adds new way of mutation
        /// </summary>
        public void AddMutation(IMutation<Creature> mutation)
        {
            mutation.MutationRate = MutationRate;
            mutations.Add(mutation);

            implementedMeansOfReproduction.Add(MeanOfReproduction.mutation);
        }

        private List<MeanOfReproduction> implementedMeansOfReproduction = new List<MeanOfReproduction>();


        /// <summary>
        /// Returns set of best creatures based on their rating. (Performs sorting)
        /// </summary>
        /// <param name="count">Size of returned IEnumerable</param>
        public IEnumerable<RatedCreature<Creature>> GetBestRatedCreatures(int count)
        {
            return Selector.GetBestCreatures(count);
        }

        private ComputationManager<Creature> computationManager;

        /// <summary>
        /// Runs evolution 
        /// Before calling this method it is nessesary that following components are present:
        ///     Mandatory (throws exeption...):
        ///     Selector (or at least fitness function passed to "CreateAndSetDefaultSelector")
        ///     FitnessFunctionFactory (or at least call CreateAndSetDefaultFitnessFunctionFactory with desired parametres)
        /// 
        ///     Optional (but good idea to set):
        ///     SizeOfPupulation
        ///     NumberOfSurvivals
        /// </summary>
        public void RunEvolution(int numOfSteps)
        {
            if (Selector == null)
                throw new UnassignedSelectorException();

            if (!ThereIsAtLeastOneWayToReproduce()) //ToDo user dont have to implement all of it
                throw new SomeMeanOfReproductionHasNotBeenProvidedException();

            for (int i = 0; i < numOfSteps; i++)
            {
                computationManager.RunOneGeneration();
                NumOfGenerationSoFar++;
            }
        }

        private bool ThereIsAtLeastOneWayToReproduce()
            => mutations.Count != 0 || sexualReproductions.Count != 0 || asexualReproductions.Count != 0;

        internal IRandomReproductionPicker<Creature> GetRandomReproductionPicker()
            => new RandomReproductionPicker<Creature>(this, rnd.Next());

        private class RandomReproductionPicker<CreatureOfRepPicker> : IRandomReproductionPicker<CreatureOfRepPicker>
        {
            EnvironmentOf<CreatureOfRepPicker> environment;
            Random rnd;

            public RandomReproductionPicker(EnvironmentOf<CreatureOfRepPicker> environment, int seed)
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

            public MeanOfReproduction GetRandomMeanOfReproduction()
            {
                int rndIndex = rnd.Next(0, environment.implementedMeansOfReproduction.Count);

                return environment.implementedMeansOfReproduction[rndIndex];
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
    class UnassignedSelectorException : Exception { } 
    class SomeMeanOfReproductionHasNotBeenProvidedException : Exception { }
}
