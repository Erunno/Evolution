using System;
using System.Collections.Generic;
using System.Text;

namespace Evolution.Users_interfaces
{
    public interface IMutation<Creature>
    {
        /// <summary>
        /// Slightly changes creature in argument
        /// </summary>
        /// <returns>"Child" of creture</returns>
        Creature GetMutatedCreature(Creature parent);

        /// <summary>
        /// Use to determine how "big" the change should be.
        /// Is in interval [0,1]
        /// </summary>
        double MutationRate { get; set; }
    }

    public interface IAsexualReproduction<Creature>
    {
        /// <summary>
        /// Creates new creture based on parent
        /// </summary>
        Creature GetChildOf(Creature parent);
    }

    public interface ISexualReproduction<Creature>
    {
        /// <summary>
        /// Creates new creature based on two parents
        /// </summary>
        Creature GetChildOf(Creature parent1, Creature parent2);
    }
}
