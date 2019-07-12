using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution
{
    class MutationManager<Creature>
    {
        private StartPool<Creature> soursePool;

        private EnvironmentOf<Creature> myEnvironment;

        private IReproductionPicker<Creature> reproductionPicker;

        public MutationManager(EnvironmentOf<Creature> environment, StartPool<Creature> startPool, Creature foreFather)
        {
            soursePool = startPool;
            myEnvironment = environment;

            reproductionPicker = environment.GetReproductionPicker();
            lastCreature = foreFather;
        }

        private Creature lastCreature;
        
        /// <summary>
        /// Returns collection of creatures which has been created from survivals
        /// </summary>
        public IEnumerable<Creature> GetChildren()
        {
            while(true)
            {
                Creature CreatureToProcess;

                lock (soursePool)
                    if (soursePool.IsEmpty)
                        break;
                    else
                        CreatureToProcess = soursePool.GetNextCreature().TheCreature;

                foreach (var child in GenerateAllChildrenOfMainParent(mainParent: CreatureToProcess, secondParent: lastCreature))
                    yield return child;

                lastCreature = CreatureToProcess;
            }
        }
        
        private IEnumerable<Creature> GenerateAllChildrenOfMainParent(Creature mainParent, Creature secondParent)
        {
            int rndCount = GetNumberOfChildren();

            for (int i = 0; i < rndCount; i++)
                switch (reproductionPicker.GetRandomMeanOfReproduction())
                {
                    case MeanOfReproduction.sexual:
                        yield return GetRandomChild(mainParent, secondParent);
                        break;
                    case MeanOfReproduction.asexual:
                        yield return GetRandomChild(mainParent);
                        break;
                    case MeanOfReproduction.mutation:
                        yield return GetRandomMutant(mainParent);
                        break;
                    default:
                        throw new NotImplementedException();
                }
        }

        private Creature GetRandomMutant(Creature creature)
        {
            return reproductionPicker.GetRandomMutation().GetMutatedCreature(creature);
        }
        private Creature GetRandomChild(Creature parent)
        {
            return reproductionPicker.GetRandomAsexualReproduction().GetChildOf(parent);
        }
        private Creature GetRandomChild(Creature parent1, Creature parent2)
        {
            return reproductionPicker.GetRandomSexualReproduction().GetChildOf(parent1, parent2);
        }

        private int GetNumberOfChildren() => reproductionPicker.GetNumOfChildren();
    }
}
