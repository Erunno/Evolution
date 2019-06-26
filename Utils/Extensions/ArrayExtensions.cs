using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public static class ArrayExtensions
    {
        public static void Swap<T>(this T[] array, int i, int j)
        {
            T tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }
    }
}
