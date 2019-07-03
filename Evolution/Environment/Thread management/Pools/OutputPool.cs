using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class OutputPool<Creature> //TODO implement OutputPool
    {

        Queue<RatedCreature<Creature>> creatures = new Queue<RatedCreature<Creature>>();

        public Pulser IsNotEmpty = new Pulser();


        public void SaveRatedCreature(RatedCreature<Creature> creature)
        {
            creatures.Enqueue(creature);

            if (creatures.Count == 1)
                lock (IsNotEmpty)
                    Monitor.Pulse(IsNotEmpty);
        }

        public RatedCreature<Creature> GetCreature() => creatures.Count != 0 ? creatures.Dequeue() : throw new PoolIsEmptyException();
    }
}
