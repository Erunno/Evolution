using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Evolution
{
    class ComputationManager<Creature>
    {
        private List<PairPC<Creature>> ActiveJobs;

        private StartPool<Creature> startPool;
        private InterPool<Creature> interPool;
        private OutputPool<Creature> outputPool;

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
