using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution.Environment.Thread_management
{
    class StartPool<Creature>
    {
        public bool IsEmpty { get; private set; }

        private IEnumerable<RatedCreature<Creature>> creaturesCollection;
        private IEnumerator<RatedCreature<Creature>> enumeratorOfCreatures;
        
        /// <summary>
        /// Return next creature and throws exception if set is empty
        /// </summary>
        public RatedCreature<Creature> GetNextCreature()
        {
            if (IsEmpty)
                throw new PoolIsEmptyException();

            RatedCreature<Creature> toReturn = enumeratorOfCreatures.Current;
            IsEmpty = !enumeratorOfCreatures.MoveNext();

            return toReturn;
        }

        public void FillWithNewCreatures(IEnumerable<RatedCreature<Creature>> newCollection)
        {
            creaturesCollection = newCollection;
            enumeratorOfCreatures = creaturesCollection.GetEnumerator();
            IsEmpty = !enumeratorOfCreatures.MoveNext();
        }
    }

    class PoolIsEmptyException : Exception { }
}
