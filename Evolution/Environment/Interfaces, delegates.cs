using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution
{

    /// <summary>
    /// Is called when new best creature is found
    /// </summary>
    public delegate void NewBestCretureFoundEventDelegate<Creature>(Creature newBestCreature, double FitnessValue);

    /// <summary>
    /// Creates new instatnce which implements IMutation
    /// </summary>
    public delegate IMutation<Creature> MutationProvider<Creature>();

    /// <summary>
    /// Creates new instatnce which implements ISexualReproduction
    /// </summary>
    public delegate ISexualReproduction<Creature> SexualReproductionProvider<Creature>();

    /// <summary>
    /// Creates new instatnce which implements IAsexualReproduction
    /// </summary>
    public delegate IAsexualReproduction<Creature> AsexualReproductionProvider<Creature>();
}
