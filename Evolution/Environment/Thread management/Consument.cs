using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution.Environment.Thread_management
{
    class ConsumentRater<Creature>
    {
        private OutputPool<Creature> outputPool;
        private InterPool<Creature> sourcePool;
        private Connection connectionToProducent;

        /// <summary>
        /// Entry point of ConsumentRater
        /// 
        /// When source of base creatures dries out ConsumentRater will fall asleep
        /// </summary>
        public void Run()
        {
            throw new NotImplementedException();
        }

    }
}
