using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using System.Threading;

namespace Evolution
{
    class ComputationManager<Creature>
    {
        private List<ThreadJobPair<Creature>> activeJobs;

        private EnvironmentOf<Creature> myEnvironment;

        private StartPool<Creature> startPool = new StartPool<Creature>();

        public ComputationManager(EnvironmentOf<Creature> environment)
        {
            myEnvironment = environment;

            Creature foreFather = GetForeFather();

            CreateComputationCores(foreFather);
        }

        private void CreateComputationCores(Creature foreFather)
        {
            activeJobs = new List<ThreadJobPair<Creature>>();

            for (int i = 0; i < myEnvironment.MaximalNumOfRunningThreads; i++)
            {
                ComputationCore<Creature> core = new ComputationCore<Creature>(myEnvironment, startPool, foreFather);
                core.WorkHasBeenDoneDelegate = SourseDriedOutHandler;

                ThreadJobPair<Creature> threadJob = new ThreadJobPair<Creature>(core);
                activeJobs.Add(threadJob);
            }
        }

        private Creature GetForeFather()
        {
            try
            {
                return myEnvironment.Selector.PeekBestCreature().TheCreature;
            }
            catch(EmptyHeapExeption)
            {
                throw new NoForeFatherException();
            }

        }

        public void UpdateFitnessFunction()
        {
            foreach (var core in activeJobs)
                core.ComputationCore.CurrentFitnessFunction = myEnvironment.FitnessFunctionFactory.CreateNewFitnessFunctionDelegate();
        }

        public void RunOneGeneration()
        {
            InitNewRun();

            FillStartPool();
            WakeUpAllThreads();

            WaitUntilJobIsDone();
        }

        private void InitNewRun()
        {
            workingThreads = activeJobs.Count;
        }

        private void FillStartPool()
        {
            IEnumerable<RatedCreature<Creature>> bestCreatures = myEnvironment.Selector.GetBestCreatures(myEnvironment.NumberOfSurvivals);

            startPool.FillWithNewCreatures(bestCreatures);
        }

        private void WakeUpAllThreads()
        {
            foreach (var threadJob in activeJobs)
                lock (threadJob.ComputationCore.BackToWorkPulser)
                    Monitor.Pulse(threadJob.ComputationCore.BackToWorkPulser);
        }

        private void WaitUntilJobIsDone()
        {
            lock (JobIsDonePulser)
                Monitor.Wait(JobIsDonePulser);
        }

        private Pulser JobIsDonePulser { get; } = new Pulser();
        private int workingThreads;

        private void SourseDriedOutHandler()
        {
            lock (JobIsDonePulser)
            {
                workingThreads--;

                if (workingThreads == 0)
                    Monitor.PulseAll(JobIsDonePulser);
            }
        }
    }

    class ThreadJobPair<Creature>
    {
        public Thread Thread { get; }
        public ComputationCore<Creature> ComputationCore { get; }

        public ThreadJobPair(ComputationCore<Creature> computationCore)
        {
            ComputationCore = computationCore;

            Thread = new Thread(ComputationCore.Run);
            Thread.Start();
        }
    }

    class NoForeFatherException : Exception { }
}
