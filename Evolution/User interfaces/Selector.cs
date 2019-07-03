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

        FitnessFunctionDelegate<Creature> FitnessFunction { get; }
    }

    /// <summary>
    /// Function which evaluates a crature
    /// </summary>
    public delegate double FitnessFunctionDelegate<Creature>(Creature creture);

    class DefaultSelector<Creature> : ISelector<Creature> //ToDo implement default selector
    {
        public FitnessFunctionDelegate<Creature> FitnessFunction { get; }

        HeapOfMaximalSize<RatedCreature<Creature>> heap;

        public DefaultSelector(FitnessFunctionDelegate<Creature> fitnessFunction, int sizeOfSetOfReturnedCreatures)
        {
            FitnessFunction = fitnessFunction;
            heap = new HeapOfMaximalSize<RatedCreature<Creature>>(sizeOfSetOfReturnedCreatures);
            bestCreatures = new RatedCreature<Creature>[sizeOfSetOfReturnedCreatures];
        }

        public void AddCreature(RatedCreature<Creature> newCreature)
        {
            if (heap.IsFull)
            {
                if (newCreature.CompareTo(heap.PeekMin()) > 0) //if newCreature is better than the worst of creatures in heap
                {
                    heap.ExtractMin();
                    heap.Insert(newCreature);                  
                }
            }
            else
                heap.Insert(newCreature);
        }

        private RatedCreature<Creature>[] bestCreatures;
        int versionOfBestCreatures = 0;

        public IEnumerable<RatedCreature<Creature>> GetBestCreatures(int count)
        {
            if (heap.Count < count)
                throw new NotEnoughElementsException();

            heap.CopyHeapToArray(bestCreatures);

            versionOfBestCreatures++;
            Array.Sort(bestCreatures, index: 0, length: heap.Count);

            return GetRatedCreatures_IterMethod(count,versionOfBestCreatures);
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
                heap.ExtractMin();
        } 
    }

    public class NotEnoughElementsException : Exception { }
}
