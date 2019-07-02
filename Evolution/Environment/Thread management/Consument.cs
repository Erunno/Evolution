using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution.Environment.Thread_management
{
    class Consument<Creature>
    {
        private EnvironmentOf<Creature> myEnvironment;
        private InterPool<Creature> sourcePool;

        /// <summary>
        /// Entry point of Consument
        /// 
        /// When source of base creatures dries out Consument will fall asleep
        /// </summary>
        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}
