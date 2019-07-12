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
        }

        int CountToCatastrofy = 100;

        public void RunOneStepOfEvolution()
        {
            if (--CountToCatastrofy == 0)
                MakeCatastrofy();

            computationManager.RunOneGeneration();
        }

        private void MakeCatastrofy()
        {
            
        }
    }
}
