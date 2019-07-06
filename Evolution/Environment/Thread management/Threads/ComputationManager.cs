using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class ComputationManager<Creature>
    {
        private List<PairPC<Creature>> ActiveJobs;

        private List<PairPC<Creature>> ActiveProducents = new List<PairPC<Creature>>();
        private List<PairPC<Creature>> ActiveConsuments = new List<PairPC<Creature>>();

        private StartPool<Creature> startPool;
        private InterPool<Creature> interPool;
        private OutputPool<Creature> outputPool;

        private EnvironmentOf<Creature> myEnvironment;

        private OutputManager<Creature> outputManager;
        private Thread threadOfOutputManager;

        public ComputationManager(EnvironmentOf<Creature> environment)
        {
            myEnvironment = environment;

            CreateOutputManagerAndHisThread();
            threadOfOutputManager.Start();

            CreatePools();

            CreateJobs();
            StartJobs();
        }

        private void CreateJobs()
        {
            ActiveJobs = new List<PairPC<Creature>>();
            for (int i = 0; i < myEnvironment.MaximalNumOfRunningThreads; i++)
            {
                Producent<Creature> prod = new Producent<Creature>(myEnvironment);
                prod.SourcePool = startPool;
                prod.TargetPool = interPool;

                ConsumentRater<Creature> cons = new ConsumentRater<Creature>(myEnvironment);
                cons.SourcePool = interPool;
                cons.TargetPool = outputPool;

                ActiveJobs.Add(new PairPC<Creature>(prod, cons));
            }
        }

        private void CreatePools()
        {
            startPool = new StartPool<Creature>();
            interPool = new InterPool<Creature>();
            outputPool = new OutputPool<Creature>();
        }

        private void CreateOutputManagerAndHisThread()
        {
            outputManager = new OutputManager<Creature>();
            outputManager.myEnvironment = myEnvironment;
            outputManager.RatedCreatures = outputPool;

            threadOfOutputManager = new Thread(outputManager.Run);
        }

        private void StartJobs()
        {
            MarkAllAsProducents();

            foreach (var pair in ActiveJobs)
            {
                pair.ConsumerThread.Start();
                pair.ProducentThread.Start();
            }
        }

        private void MarkAllAsProducents()
        {
            ActiveConsuments.Clear();

            foreach (var pair in ActiveJobs)
                ActiveProducents.Add(pair);
        }

        public void RunOneGeneration(IEnumerable<RatedCreature<Creature>> bestCreatures)
        {
            MarkAllAsProducents();
            startPool.FillWithNewCreatures(bestCreatures);

            lock (startPool)
                Monitor.PulseAll(startPool); //wake up all producents

            ManageProducentsAndConsuments();

            WaitToTheEnd();
        }

        private void ManageProducentsAndConsuments()
        {
            throw new NotImplementedException();
        }

        private void WaitToTheEnd()
        {

        }
    }

    class PairPC<Creature>
    {
        public Producent<Creature> Producent { get; }
        public Thread ProducentThread { get; }

        public ConsumentRater<Creature> Consument { get; }
        public Thread ConsumerThread { get; }

        public Connection Connection { get; }

        public PairPC(Producent<Creature> prod, ConsumentRater<Creature> cons)
        {
            Producent = prod;
            Consument = cons;
            Connection = new Connection();

            Producent.ConnectionToConsument = Connection;
            Consument.ConnectionToProducent = Connection;

            ConsumerThread = new Thread(Consument.Run);
            ProducentThread = new Thread(Producent.Run);
        }

    }
}
