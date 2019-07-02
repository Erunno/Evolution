using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution.Environment.Thread_management
{
    class StartPool<Creature>
    {
        public int RemainingCratures { get; private set; }
        public bool IsEmpty => RemainingCratures == 0;

        /// <summary>
        /// Return next creature and throws exception if set is empty
        /// </summary>
        /// <returns></returns>
        public Creature GetNextCreature()
        {
            throw new NotImplementedException();
        }
    }
}
