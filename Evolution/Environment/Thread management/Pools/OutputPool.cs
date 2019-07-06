using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class OutputPool<Creature> //TODO implement OutputPool
    {

        Queue<RatedCreature<Creature>> creatures = new Queue<RatedCreature<Creature>>();

        public bool IsEmty => creatures.Count == 0;

        public void SaveRatedCreature(RatedCreature<Creature> creature)
        {
            creatures.Enqueue(creature);

            if (creatures.Count == 1)
                lock (this)
                    Monitor.Pulse(this);
        }

        public RatedCreature<Creature> GetCreature() => creatures.Count != 0 ? creatures.Dequeue() : throw new PoolIsEmptyException();
    }
}
