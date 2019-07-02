using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution.Environment.Thread_management
{
    class Producent<Creature>
    {
        private EnvironmentOf<Creature> myEnvironment;
        private InterPool<Creature> targetPool;
        private StartPool<Creature> sourcePool;
        
        /// <summary>
        /// Starting point of producent
        /// 
        /// When source of base creatures dries out producent will fall asleep
        /// </summary>
        public void Run()
        {
            throw new NotImplementedException();
        } 
    }
}
