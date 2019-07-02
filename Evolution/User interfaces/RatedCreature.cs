namespace Evolution
{
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

}