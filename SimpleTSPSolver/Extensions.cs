using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTSPSolver
{
    static class Extensions
    {
        public static void Swich<T>(this T[] array, int i, int j)
        {
            T tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }
    }
}
