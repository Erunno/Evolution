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

        public static void CopyTo<T>(this T[] array, T[] targer)
        {
            if (targer.Length != array.Length)
                throw new ArgumentException();

            for (int i = 0; i < array.Length; i++)
                targer[i] = array[i];
        }
    }
}
