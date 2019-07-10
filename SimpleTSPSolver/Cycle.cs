using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTSPSolver
{
    class Cycle
    {
        public int[] Verticies;

        public Cycle(int length)
        {
            Verticies = new int[length];
        }

        public Cycle(int[] Verticies)
        {
            this.Verticies = Verticies;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(); ;

            for (int i = 0; i < Verticies.Length; i++)
                sb.Append(Verticies[i] + " ");

            return sb.ToString();
        }
    }
}
