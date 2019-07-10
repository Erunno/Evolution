using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution
{
    public class DisposedCreaturesStore<Creature>
    {
        public bool StoreCreatures { get; private set; } = false;

        public bool IsEmpty { get; private set; }

        /// <summary>
        /// Sets StoreCreatures to false and stored creatures will be fotgoten 
        /// </summary>
        public void  UnsetStoreCraeturesAndForgetStoredCreatures()
        {
            disposedCreatures = null;
            StoreCreatures = false;
        }

        /// <summary>
        /// Sets StoreCreatures to true
        /// </summary>
        public void SetStoreCreatures()
        {
            disposedCreatures = new Queue<Creature>();
            StoreCreatures = true;
        }

        private Queue<Creature> disposedCreatures;

        /// <summary>
        /// Adds new disposed creature
        /// 
        /// Thread save method
        /// </summary>
        public void EmplaceCreature(Creature disposedCreature)
        {
            if (!StoreCreatures)
                throw new NotStoringDisposedCreaturesException();

            lock (disposedCreature)
                disposedCreatures.Enqueue(disposedCreature);
        }

        /// <summary>
        /// Returned creature is removed from the set
        /// 
        /// Thread save method
        /// </summary>
        public Creature ExtractCreature()
        {
            if (!StoreCreatures)
                throw new NotStoringDisposedCreaturesException();

            if (IsEmpty)
                throw new EmptyStoreException();

            IsEmpty = disposedCreatures.Count == 1; //after returnnig there will be none

            lock (disposedCreatures)
                return disposedCreatures.Dequeue();
        } 
    }

    class NotStoringDisposedCreaturesException : Exception { }
    class EmptyStoreException : Exception { }
}
