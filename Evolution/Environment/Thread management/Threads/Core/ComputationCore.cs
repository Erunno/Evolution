using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class ComputationCore<Creature>
    {
        private MutationManager<Creature> mutationManager;

        private EnvironmentOf<Creature> myEnvironment;

        public Pulser BackToWorkPulser { get; } = new Pulser();
        public bool IsWorking { get; private set; } = false;

        public Action WorkHasBeenDoneDelegate;

        public ComputationCore(EnvironmentOf<Creature> environment, StartPool<Creature> startPool, Creature foreFather)
        {
            mutationManager = new MutationManager<Creature>(environment, startPool, foreFather);
        }

        /// <summary>
        /// Entry point of Computation core
        /// </summary>
        public void Run()
        {
            lock (BackToWorkPulser)
                Monitor.Wait(BackToWorkPulser);

            while(true)
            {
                lock(BackToWorkPulser)
                    IsWorking = true;

                ComputeOneGenaration();

                lock (BackToWorkPulser)
                {
                    IsWorking = false;
                    WorkHasBeenDoneDelegate?.Invoke();

                    Monitor.Wait(BackToWorkPulser);
                }
            }
        }

        private void ComputeOneGenaration()
        {
            IEnumerable<Creature> newGeneration = mutationManager.GetChildren();

            foreach (var child in newGeneration)
            {
                RatedCreature<Creature> evaluatedChild = EvaluateCreature(child);
                AddCreatureToSelector(evaluatedChild);
            }
        }

        private RatedCreature<Creature> EvaluateCreature(Creature creature)
        {
            double fitnessValue = myEnvironment.Selector.FitnessFunction(creature);

            return new RatedCreature<Creature>(creature, fitnessValue);
        }

        private void AddCreatureToSelector(RatedCreature<Creature> creature)
        {
            lock (myEnvironment.Selector)
                myEnvironment.Selector.AddCreature(creature);
        }
    }
}
