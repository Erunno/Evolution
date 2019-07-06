using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class ConsumentRater<Creature>
    {
        public OutputPool<Creature> TargetPool { get; set; }
        public InterPool<Creature> SourcePool { get; set; }

        public Connection ConnectionToProducent { get; set; }
        private EnvironmentOf<Creature> myEnvironment;

        public ConsumentRater(EnvironmentOf<Creature> environment)
        {
            myEnvironment = environment;
        }

        /// <summary>
        /// Entry point of ConsumentRater
        /// 
        /// When source of base creatures dries out ConsumentRater will fall asleep
        /// </summary>
        public void Run()
        {
            lock (ConnectionToProducent)
                Monitor.Wait(ConnectionToProducent);

            while (true)
                EvaluateOneCreature();
        }

        private void EvaluateOneCreature()
        {
            Creature creatureToEvaluate = TryGetCreatureOrFallAsleep();

            double fitnessValue = myEnvironment.Selector.FitnessFunction(creatureToEvaluate);

            lock (TargetPool)
                TargetPool.SaveRatedCreature(new RatedCreature<Creature>(creatureToEvaluate, fitnessValue));
        }

        private Creature TryGetCreatureOrFallAsleep()
        {
            ConnectionToProducent.TryGiveOtherProcessingTime();

            lock (SourcePool)
            {
                while (SourcePool.IsEmpty)
                    Monitor.Wait(SourcePool);

                return SourcePool.ExtractCreature();
            }
        }
    }
}
