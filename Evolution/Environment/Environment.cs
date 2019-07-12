using System;
using System.Collections.Generic;
using System.Linq;

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
            Selector.SetMaximalNuberOfSurvivals(startingInfo.NumberOfSurvivals);

            FitnessFunctionFactory = startingInfo.GetFitnessFunctionFactory();

            interval = new IntervalOfChildrenCount<Creature>(this);

            SizeOfPupulation = startingInfo.SizeOfPopulation;
            MutationRate = startingInfo.MutationRate;
            MaximalNumOfRunningThreads = startingInfo.NumberOfRunningThreads;

            computationManager = new ComputationManager<Creature>(this);
            UpdateComputationManager();

            evolutionManager = new EvolutionManager<Creature>(this, computationManager);
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
            UpdateComputationManager();
        }

        private void UpdateComputationManager()
        {
            computationManager.UpdateFitnessFunction();
        }

        Random rnd = new Random();

        private List<MutationProvider<Creature>> mutationProviders { get; } = new List<MutationProvider<Creature>>();
        private List<SexualReproductionProvider<Creature>> sexualReproductionProviders { get; } = new List<SexualReproductionProvider<Creature>>();
        private List<AsexualReproductionProvider<Creature>> asexualReproductionProviders { get; } = new List<AsexualReproductionProvider<Creature>>();
        private List<IReproductionStore<Creature>> reproductionStores { get; } = new List<IReproductionStore<Creature>>();

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
        public void SetMutationRate(double newRate)
        {
            if (newRate < 0 || 1 < newRate)
                throw new UnvalidMutationRateException();

            foreach(var store in reproductionStores)
                foreach (var mutation in store.Mutations)
                    mutation.MutationRate = newRate;

            MutationRate = newRate;
        }

        //todo change interface and add providers

        /// <summary>
        /// Adds new way of asexual reproduction.
        /// </summary>
        /// <param name="asexualReproduction">Its methods have to thread save since it can be used simultaneously</param>
        public void AddAsexualReproduction(IAsexualReproduction<Creature> asexualReproduction)
            => AddAsexualReproductionProvider(() => asexualReproduction);

        /// <summary>
        /// Adds new way of sexual reproduction
        /// </summary>
        /// <param name="sexualReproduction">Its methods have to thread save since it can be used simultaneously</param>
        public void AddSexualReproduction(ISexualReproduction<Creature> sexualReproduction)
            => AddSexualReproductionProvider(() => sexualReproduction);

        /// <summary>
        /// Adds new way of mutation
        /// </summary>
        /// <param name="mutation">Its methods have to thread save since it can be used simultaneously</param>
        public void AddMutation(IMutation<Creature> mutation)
            => AddMutationProvider(() => mutation);

        /// <summary>
        /// Add delegate which provides instance which implements IMutation.
        /// The instance does not have to thread save because every thread will have its own.
        /// </summary>
        public void AddMutationProvider(MutationProvider<Creature> mutationProvider)
        {
            mutationProviders.Add(mutationProvider);

            foreach (var store in reproductionStores)
                store.Mutations.Add(mutationProvider());

            implementedMeansOfReproduction.Add(MeanOfReproduction.mutation);
        }

        /// <summary>
        /// Add delegate which provides instance which implements ISexualReproduction.
        /// The instance does not have to thread save because every thread will have its own.
        /// </summary>
        public void AddSexualReproductionProvider(SexualReproductionProvider<Creature> sexualReproductionProvider)
        {
            sexualReproductionProviders.Add(sexualReproductionProvider);

            foreach (var store in reproductionStores)
                store.SexualReproductions.Add(sexualReproductionProvider());

            implementedMeansOfReproduction.Add(MeanOfReproduction.sexual);
        }

        /// <summary>
        /// Add delegate which provides instance which implements IAsexualReproduction.
        /// The instance does not have to thread save because every thread will have its own.
        /// </summary>
        public void AddAsexualReproductionProvider(AsexualReproductionProvider<Creature> asexualReproductionProvider)
        {
            asexualReproductionProviders.Add(asexualReproductionProvider);

            foreach (var store in reproductionStores)
                store.AsexualReproductions.Add(asexualReproductionProvider());

            implementedMeansOfReproduction.Add(MeanOfReproduction.asexual);
        }

        
        private List<MeanOfReproduction> implementedMeansOfReproduction = new List<MeanOfReproduction>();


        /// <summary>
        /// Returns set of best creatures based on their rating. (Performs sorting)
        /// </summary>
        /// <param name="count">Size of returned IEnumerable</param>
        public IEnumerable<RatedCreature<Creature>> GetBestRatedCreatures(int count)
        {
            var orderedCreatures =
                from cr in Selector.GetSurvivingCreatures()
                orderby cr.FitnessValue descending
                select cr;

            return orderedCreatures.Take(count);
        }

        private ComputationManager<Creature> computationManager { get; }

        private EvolutionManager<Creature> evolutionManager { get; }

        /// <summary>
        /// Runs evolution 
        /// Before calling this method it is nessesary that following components are present:
        ///     Mandatory (throws exeption...):
        ///     Selector (or at least fitness function passed to "CreateAndSetDefaultSelector")
        ///     FitnessFunctionFactory (or at least call CreateAndSetDefaultFitnessFunctionFactory with desired parametres)
        /// </summary>
        public void RunEvolution(int numOfSteps)
        {
            if (Selector == null) //todo check whether i check everything
                throw new UnassignedSelectorException();

            if (!ThereIsAtLeastOneWayToReproduce())
                throw new SomeMeanOfReproductionHasNotBeenProvidedException();

            interval.RecomputeInterval();

            for (int i = 0; i < numOfSteps; i++)
            {
                evolutionManager.RunOneStepOfEvolution();
                NumOfGenerationSoFar++;
            }
        }

        private bool ThereIsAtLeastOneWayToReproduce()
            => mutationProviders.Count != 0 || 
            sexualReproductionProviders.Count != 0 || 
            asexualReproductionProviders.Count != 0;

        internal IReproductionPicker<Creature> GetReproductionPicker()
        {
            var newReproductionPicker = new RandomReproductionPicker<Creature>(environment: this, seed: rnd.Next());
            reproductionStores.Add(newReproductionPicker);

            FillReproductionStore(newReproductionPicker);

            return newReproductionPicker;
        }

        private void FillReproductionStore(IReproductionStore<Creature> store)
        {
            foreach (var mutationProvider in mutationProviders)
                store.Mutations.Add(mutationProvider());

            foreach (var asexualReproductionProvider in asexualReproductionProviders)
                store.AsexualReproductions.Add(asexualReproductionProvider());

            foreach (var sexualReproductionProvider in sexualReproductionProviders)
                store.SexualReproductions.Add(sexualReproductionProvider());
        }

        private interface IReproductionStore<CreatureOfInterface>
        {
            List<IMutation<CreatureOfInterface>> Mutations { get; }
            List<ISexualReproduction<CreatureOfInterface>> SexualReproductions { get; }
            List<IAsexualReproduction<CreatureOfInterface>> AsexualReproductions { get; }
        }

        private class RandomReproductionPicker<CreatureOfRepPicker> : IReproductionPicker<CreatureOfRepPicker>, IReproductionStore<CreatureOfRepPicker>
        {
            EnvironmentOf<CreatureOfRepPicker> environment;
            Random rnd;

            List<IMutation<CreatureOfRepPicker>> IReproductionStore<CreatureOfRepPicker>.Mutations { get; } 
                = new List<IMutation<CreatureOfRepPicker>>();

            List<ISexualReproduction<CreatureOfRepPicker>> IReproductionStore<CreatureOfRepPicker>.SexualReproductions { get; } 
                = new List<ISexualReproduction<CreatureOfRepPicker>>();

            List<IAsexualReproduction<CreatureOfRepPicker>> IReproductionStore<CreatureOfRepPicker>.AsexualReproductions { get; } 
                = new List<IAsexualReproduction<CreatureOfRepPicker>>();

            public RandomReproductionPicker(EnvironmentOf<CreatureOfRepPicker> environment, int seed)
            {
                this.environment = environment;
                rnd = new Random(seed);
            }

            public IMutation<CreatureOfRepPicker> GetRandomMutation()
            {
                var Mutations = ((IReproductionStore<CreatureOfRepPicker>)this).Mutations;

                return GetRandomElement(Mutations);
            }

            public ISexualReproduction<CreatureOfRepPicker> GetRandomSexualReproduction()
            {
                var SexualReproductions = ((IReproductionStore<CreatureOfRepPicker>)this).SexualReproductions;

                return GetRandomElement(SexualReproductions);
            }

            public IAsexualReproduction<CreatureOfRepPicker> GetRandomAsexualReproduction()
            {
                var AsexualReproductions = ((IReproductionStore<CreatureOfRepPicker>)this).AsexualReproductions;

                return GetRandomElement(AsexualReproductions);
            }

            private T GetRandomElement<T>(List<T> list)
            {
                int randomIndex = rnd.Next(0, (list.Count));
                return list[randomIndex];
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
    interface IReproductionPicker<Creature>
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
            int numOfSurvivals = environment.Selector.NumberOfSurvivals;

            int meanValueOfChildren = (environment.SizeOfPupulation + numOfSurvivals) / numOfSurvivals; //rounded up

            From = meanValueOfChildren - Variation;
            To = meanValueOfChildren + Variation + 1; //upper bound is exclusive
        }
    }
    class UnvalidMutationRateException : Exception { }
    class UnassignedSelectorException : Exception { } 
    class SomeMeanOfReproductionHasNotBeenProvidedException : Exception { }
}
