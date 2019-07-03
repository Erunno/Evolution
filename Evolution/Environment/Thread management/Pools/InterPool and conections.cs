using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class InterPool<Creature> //TODO implement InterPool
    {

        public bool IsEmpty { get; private set; }

        public void AddCreature(Creature creature)
        {
            throw new NotImplementedException();
        }

        public Creature ExtractCreature()
        {
            throw new NotImplementedException();
        }

        public void ExtractCreaturesTo(Creature[] container)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Represent conection between Consumer and producent
    /// </summary>
    class Connection
    {
        public bool SwichThread = false;

        private object Lock = new object();

        /// <summary>
        /// Decides whether it is right time to swich from consumer/producent to prod./cons.
        /// </summary>
        public void TryGiveOtherProcessingTime()
        {
            lock (Lock)
            {
                if (SwichThread)
                {
                    Monitor.Pulse(Lock);
                    Monitor.Wait(Lock);
                }
            }
        }
    }
}
