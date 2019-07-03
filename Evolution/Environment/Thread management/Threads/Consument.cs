using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class ConsumentRater<Creature>
    {
        private OutputPool<Creature> outputPool;
        private InterPool<Creature> sourcePool;

        private Connection connectionToProducent;
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
            connectionToProducent.TryGiveOtherProcessingTime();

            lock (sourcePool)
            {
                while (sourcePool.IsEmpty)
                    Monitor.Wait(sourcePool);

                return sourcePool.ExtractCreature();
            }
        }
    }
}
