using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using System.Linq;

namespace Evolution
{
    public interface ISelector<Creature>
    {
        /// <summary>
        /// Adds creature and its value to set of creatures
        /// </summary>
        void AddCreature(RatedCreature<Creature> ratedCreature);

        /// <summary>
        /// Returns set of best creatures in the set
        /// </summary>
        IEnumerable<RatedCreature<Creature>> GetSurvivingCreatures();

        /// <summary>
        /// Fill given list with surviving creatures.
        /// Count of provided List have to bigger than or equal to NumberOfSurvivingCreatures
        /// </summary>
        /// <returns>Count of creatures with which was the list filled</returns>
        int FillWithSurvivingCreatures(List<RatedCreature<Creature>> litsToBeFilled);

        int NumberOfSurvivals { get; }

        void SetMaximalNuberOfSurvivals(int newCount);

        RatedCreature<Creature> PeekBestCreature();

        /// <summary>
        /// Returns all surviving creatures and it will drop them
        /// </summary>
        IEnumerable<RatedCreature<Creature>> ExtractSurvivingCratures();
    }


    /// <summary>
    /// Function which evaluates a crature
    /// </summary>
    public delegate double FitnessFunctionDelegate<Creature>(Creature creture);

    class DefaultSelector<Creature> : ISelector<Creature>
    {
        HeapOfMaximalSize<RatedCreature<Creature>> heap;

        public NewBestCretureFoundEventDelegate<Creature> NewBestCretureFound { get; set; }

        public int NumberOfSurvivals { get; private set; }

        private DisposedCreaturesStore<Creature> DisposedCreatures;

        public DefaultSelector(
            NewBestCretureFoundEventDelegate<Creature> newBestFound, 
            RatedCreature<Creature> foreFather, int NumberOfSurvivals,
            DisposedCreaturesStore<Creature> disposedCreatures)
        {
            NewBestCretureFound = newBestFound;
            DisposedCreatures = disposedCreatures;

            heap = new HeapOfMaximalSize<RatedCreature<Creature>>(NumberOfSurvivals);

            bestCreature = foreFather;
            AddCreature(foreFather);
        }

        public void AddCreature(RatedCreature<Creature> newCreature)
        {
            if (IsBetterThanBest(newCreature))
                HandleNewBest(newCreature);

            if (heap.IsFull)
            {
                if (newCreature.CompareTo(heap.PeekMin()) > 0) //if newCreature is better than the worst of creatures in heap
                {
                    Creature theWorst = heap.ExtractMin().TheCreature;
                    TrySaveDisposedCreature(theWorst);

                    heap.Insert(newCreature);
                }
                else
                    TrySaveDisposedCreature(newCreature.TheCreature);
            }
            else
                heap.Insert(newCreature);
        }

        private void TrySaveDisposedCreature(Creature disposedCreature)
        {
            if (DisposedCreatures.StoreCreatures)
                lock(DisposedCreatures)
                    DisposedCreatures.EmplaceCreature(disposedCreature);
        }

        private RatedCreature<Creature> bestCreature;

        private bool IsBetterThanBest(RatedCreature<Creature> creature)
            => bestCreature.CompareTo(creature) < 0; 

        private void HandleNewBest(RatedCreature<Creature> newBestCreature)
        {
            bestCreature = newBestCreature;
            NewBestCretureFound?.Invoke(newBestCreature.TheCreature, newBestCreature.FitnessValue);
        }

        public IEnumerable<RatedCreature<Creature>> GetSurvivingCreatures() => heap;

        public void SetMaximalNuberOfSurvivals(int newCount)
        {
            NumberOfSurvivals = newCount;

            if (newCount < heap.Count)
                PrepareHeapToDownSizing(newCount);

            heap.SetNewSize(newCount);
        }

        private void PrepareHeapToDownSizing(int newSize)
        {
            while(heap.Count > newSize)
            { 
                Creature theWorst = heap.ExtractMin().TheCreature;
                TrySaveDisposedCreature(theWorst);
            }
        }

        public RatedCreature<Creature> PeekBestCreature()
        {
            return bestCreature;
        }

        public IEnumerable<RatedCreature<Creature>> ExtractSurvivingCratures()
        {
            List<RatedCreature<Creature>> creaturesToReturn = heap.ToList(); //have to enumerate them, because i will modify heap

            heap.ClearWithPossibleMemoryLeaks();

            return creaturesToReturn;
        }

        public int FillWithSurvivingCreatures(List<RatedCreature<Creature>> litsToBeFilled)
        {
            if (litsToBeFilled.Count < NumberOfSurvivals)
                throw new SmallListProvidedException();

            int i = -1;
            foreach (var creature in heap)
                litsToBeFilled[++i] = creature;

            return ++i;
        }
    }

    public class NotEnoughElementsException : Exception { }

    public class SmallListProvidedException : Exception { }
}
