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
        /// Resturns set of best creature in its set
        /// </summary>
        /// <param name="count">Count of creatures in enumerable</param>
        IEnumerable<RatedCreature<Creature>> GetBestCreatures(int count);

        FitnessFunctionDelegate<Creature> FitnessFunction { get; }
    }

    /// <summary>
    /// Function which evaluates a crature
    /// </summary>
    public delegate double FitnessFunctionDelegate<Creature>(Creature creture);

    public struct RatedCreature<Creature> : IComparable<RatedCreature<Creature>>
    {
        /// <summary>
        /// Return value of fitness function
        /// </summary>
        public double FitnessValue { get; }
        public Creature TheCreature;

        public RatedCreature(Creature creature, double fitnessValue)
        {
            this.FitnessValue = fitnessValue;
            this.TheCreature = creature;
        }

        public int CompareTo(RatedCreature<Creature> other) => FitnessValue.CompareTo(other.FitnessValue);
    }

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

        public void AddCreature(RatedCreature<Creature> ratedCreature)
        {
            if (ratedCreature.CompareTo(heap.PeekMin()) < 0) //if ratedCreature is worse than every creature in heap
                return;                                      //then dont save it and return

            if (heap.IsFull)
                heap.ExtractMin(); //make space for new element

            heap.Insert(ratedCreature);
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
            for (int i = 0; i < count; i++)
            {
                if (versionOfBestCreatures != versionLocal)
                    throw new InvalidOperationException();

                yield return bestCreatures[i];
            }
        }
    }

    public class NotEnoughElementsException : Exception { }
}
