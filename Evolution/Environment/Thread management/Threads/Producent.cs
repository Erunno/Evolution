using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class Producent<Creature>
    {
        public InterPool<Creature> TargetPool { get; set; }
        public StartPool<Creature> SourcePool { get; set; }
        public Connection ConnectionToConsument { get; set; }

        private EnvironmentOf<Creature> myEnvironment;
        private IRandomReproductionPicker<Creature> reproductionPicker;

        public Producent(EnvironmentOf<Creature> environment)
        {
            myEnvironment = environment;
            reproductionPicker = myEnvironment.GetRandomReproductionPicker();
        }

        /// <summary>
        /// Starting point of producent
        /// 
        /// When source of base creatures dries out producent will fall asleep
        /// </summary>
        public void Run()
        {
            while (true)
                ProcessTwoCreatures();
        } 

        private void ProcessTwoCreatures()
        {
            Creature parent1 = GetCreaureOrFallAsleep();
            Creature parent2 = GetCreaureOrFallAsleep();

            foreach (var child in GetAllChildrenOf(parent1, parent2))
                lock (TargetPool)
                    TargetPool.AddCreature(child);

        }

        private IEnumerable<Creature> GetAllChildrenOf(Creature parent1, Creature parent2)
        {
            foreach (var child in GenerateAllChildrenOfMainParent(parent1,parent2))
                yield return child;

            foreach (var child in GenerateAllChildrenOfMainParent(parent2, parent1))
                yield return child;
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

        private Creature GetCreaureOrFallAsleep()
        {
            ConnectionToConsument.TryGiveOtherProcessingTime();

            lock (SourcePool)
            {
                while (SourcePool.IsEmpty)
                    Monitor.Wait(SourcePool);

                return SourcePool.GetNextCreature().TheCreature;
            }
        }

        private int GetNumberOfChildren() => reproductionPicker.GetNumOfChildren();
    }
}
