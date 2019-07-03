using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class ConsumentRater<Creature>
    {
        public OutputPool<Creature> outputPool { get; set; }
        public InterPool<Creature> sourcePool { get; set; }

        public Connection ConnectionToProducent { get; set; }
        private EnvironmentOf<Creature> myEnvironment;

        /// <summary>
        /// Entry point of ConsumentRater
        /// 
        /// When source of base creatures dries out ConsumentRater will fall asleep
        /// </summary>
        public void Run()
        {
            while (true)
                EvaluateOneCreature();
        }

        private void EvaluateOneCreature()
        {
            Creature creatureToEvaluate = TryGetCreatureOrFallAsleep();

            double fitnessValue = myEnvironment.Selector.FitnessFunction(creatureToEvaluate);

            lock (outputPool)
                outputPool.SaveRatedCreature(new RatedCreature<Creature>(creatureToEvaluate, fitnessValue));
        }

        private Creature TryGetCreatureOrFallAsleep()
        {
            ConnectionToProducent.TryGiveOtherProcessingTime();

            lock (sourcePool)
            {
                while (sourcePool.IsEmpty)
                    Monitor.Wait(sourcePool);

                return sourcePool.ExtractCreature();
            }
        }
    }
}
