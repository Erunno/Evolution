using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution
{

    public interface IDisposedCreaturesStore<Creature>
    {
        bool StoreCreatures { get; set; }

        /// <summary>
        /// Returned creature is removed from the set
        /// 
        /// This method is expected to be thread save
        /// </summary>
        Creature ExtractCreature();

        /// <summary>
        /// Adds new disposed creature
        /// 
        /// This method is expected to be thread save
        /// </summary>
        void EmplaceCreature(Creature disposedCreature);
    }

    class DefaultDisposedCreaturesStore<Creature> : IDisposedCreaturesStore<Creature>
    {
        private bool _storeCreatures = false;
        public bool StoreCreatures
        {
            get => _storeCreatures;
            set
            {
                if (_storeCreatures == false && value == true)
                    disposedCreatures = new Queue<Creature>();
                else if (_storeCreatures == true && value == false)
                    disposedCreatures = null;

                _storeCreatures = value;
            }
        }

        private Queue<Creature> disposedCreatures;

        public void EmplaceCreature(Creature disposedCreature)
        {
            if (!StoreCreatures)
                throw new NotStoringDisposedCreaturesException();

            lock (disposedCreature)
                disposedCreatures.Enqueue(disposedCreature);
        }

        public Creature ExtractCreature()
        {
            if (!StoreCreatures)
                throw new NotStoringDisposedCreaturesException();

            lock (disposedCreatures)
                return disposedCreatures.Dequeue();
        }
    }

    class NotStoringDisposedCreaturesException : Exception { }
}
