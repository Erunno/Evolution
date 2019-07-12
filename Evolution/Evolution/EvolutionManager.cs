using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution
{
    class EvolutionManager<Creature>
    {
        private EnvironmentOf<Creature> environment { get; }
        private ComputationManager<Creature> computationManager { get; }

        public EvolutionManager(EnvironmentOf<Creature> environment, ComputationManager<Creature> computationManager)
        {
            this.environment = environment;
            this.computationManager = computationManager;
            CountToCatastrofy = CatastrofyPeriod;
        }

        int CountToCatastrofy;
        int CatastrofyPeriod = 100;

        public void RunOneStepOfEvolution()
        {
            if (--CountToCatastrofy == 0)
            {
                MakeCatastrofy();
                CountToCatastrofy = CatastrofyPeriod;
            }
            else
                computationManager.RunOneGeneration();
        }

        private void MakeCatastrofy()
        {
            IEnumerable<RatedCreature<Creature>> parents = environment.Selector.ExtractSurvivingCratures();

            double previousMutationRate = environment.MutationRate;
            environment.SetMutationRate(0.95);

            computationManager.RunOneGeneration(parents);

            environment.SetMutationRate(previousMutationRate);
        }
    }
}
