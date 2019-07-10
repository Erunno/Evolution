using System;
using System.Collections.Generic;
using System.Text;
using Utils;

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
        /// <param name="count">Count of creatures in enumerable</param>
        IEnumerable<RatedCreature<Creature>> GetBestCreatures(int count);

        RatedCreature<Creature> PeekBestCreature();
    }


    /// <summary>
    /// Function which evaluates a crature
    /// </summary>
    public delegate double FitnessFunctionDelegate<Creature>(Creature creture);

    class DefaultSelector<Creature> : ISelector<Creature>
    {
        HeapOfMaximalSize<RatedCreature<Creature>> heap;

        public NewBestCretureFoundEventDelegate<Creature> NewBestCretureFound { get; set; }

        private DisposedCreaturesStore<Creature> DisposedCreatures;

        public DefaultSelector(
            NewBestCretureFoundEventDelegate<Creature> newBestFound, 
            RatedCreature<Creature> foreFather, int sizeOfSetOfReturnedCreatures,
            DisposedCreaturesStore<Creature> disposedCreatures)
        {
            NewBestCretureFound = newBestFound;
            DisposedCreatures = disposedCreatures;

            heap = new HeapOfMaximalSize<RatedCreature<Creature>>(sizeOfSetOfReturnedCreatures);
            bestCreatures = new RatedCreature<Creature>[sizeOfSetOfReturnedCreatures];

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
            }
            else
                heap.Insert(newCreature);
        }

        private void TrySaveDisposedCreature(Creature disposedCreature)
        {
            if (DisposedCreatures.StoreCreatures)
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

        private RatedCreature<Creature>[] bestCreatures;
        int versionOfBestCreatures = 0;

        public IEnumerable<RatedCreature<Creature>> GetBestCreatures(int count)
        {
            //if (heap.Count < count)
            //    throw new NotEnoughElementsException();
            //todo solve this

            heap.CopyHeapToArray(bestCreatures);

            versionOfBestCreatures++;
            Array.Sort(bestCreatures, index: 0, length: heap.Count);

            int realCreaturesCount = Math.Min(count, heap.Count);

            return GetRatedCreatures_IterMethod(realCreaturesCount, versionOfBestCreatures);
        }

        private IEnumerable<RatedCreature<Creature>> GetRatedCreatures_IterMethod(int count, int versionLocal)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                if (versionOfBestCreatures != versionLocal)
                    throw new InvalidOperationException();

                yield return bestCreatures[i];
            }
        }

        public void ChangeSizeOfHeap(int newSize)
        {
            if (newSize < heap.Count)
                PrepareHeapToDownSizing(newSize);

            heap.SetNewSize(newSize);
        }

        private void PrepareHeapToDownSizing(int newSize)
        {
            for (int i = 0; i < heap.Count - newSize; i++)
            {
                Creature theWorst = heap.ExtractMin().TheCreature;
                TrySaveDisposedCreature(theWorst);
            }
        }

        public RatedCreature<Creature> PeekBestCreature()
        {
            return bestCreature;
        }
    }

    public class NotEnoughElementsException : Exception { }
}
