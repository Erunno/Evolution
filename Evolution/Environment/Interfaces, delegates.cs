using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution
{

    /// <summary>
    /// Is called when new best creature is found
    /// </summary>
    public delegate void NewBestCretureFoundEventDelegate<Creature>(Creature newBestCreature, double FitnessValue);
}
