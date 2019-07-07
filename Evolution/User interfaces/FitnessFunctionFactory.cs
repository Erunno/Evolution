using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution
{

    public interface IFitnessFunctionFactory<Creature>
    {
        /// <summary>
        /// Creates new instance of FitnessFunctionDelegate.
        /// </summary>
        FitnessFunctionDelegate<Creature> CreateNewFitnessFunctionDelegate();
    }

    public delegate FitnessFunctionDelegate<Creature> FitnessFunctionProvider<Creature>();

    class DefaultFitnessFunctionFactory<Creature> : IFitnessFunctionFactory<Creature>
    {
        public DefaultFitnessFunctionFactory(FitnessFunctionDelegate<Creature> fitnessFunction)
        {
            provider = () => fitnessFunction;
        }

        public DefaultFitnessFunctionFactory(FitnessFunctionProvider<Creature> provider)
        {
            this.provider = provider;
        }

        FitnessFunctionProvider<Creature> provider;

        public FitnessFunctionDelegate<Creature> CreateNewFitnessFunctionDelegate() => provider();
    }
}
