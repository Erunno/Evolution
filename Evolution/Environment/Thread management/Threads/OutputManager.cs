using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class OutputManager<Creature>
    {
        public OutputPool<Creature> RatedCreatures { get; set; }
        public EnvironmentOf<Creature> myEnvironment { get; set; }

        /// <summary>
        /// Entry point of OutputManager
        /// </summary>
        public void Run()
        {
            while (true)
                ProcessOneCreature();
        }

        private void ProcessOneCreature()
        {
            RatedCreature<Creature> creatureToProcess = TryGetCreatureOrFallAsleep();

            myEnvironment.Selector.AddCreature(creatureToProcess);
        }

        private RatedCreature<Creature> TryGetCreatureOrFallAsleep()
        {
            lock (RatedCreatures)
            {
                while (RatedCreatures.IsEmty)
                    Monitor.Wait(RatedCreatures.IsNotEmptyPulser);

                return RatedCreatures.GetCreature();
            }
        }

    }
}
