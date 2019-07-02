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
        }

        public void AddCreature(RatedCreature<Creature> ratedCreature)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RatedCreature<Creature>> GetBestCreatures(int count)
        {
            throw new NotImplementedException();
        }
    }
}
