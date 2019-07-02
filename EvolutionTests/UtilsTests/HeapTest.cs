using NUnit.Framework;
using Utils;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

namespace UtilsTests
{
    public class HeapTests
    {
        [Test]
        [TestCase(42,7)]
        [TestCase(21, 70)]
        [TestCase(84, 700)]
        [TestCase(3, 420)]
        [TestCase(69, 2000)]
        public void Insert_ExtractMin_ProperUseOfHeap_NoExceptionsExpected(int randomSeed, int size)
        {
            //arrange
            var heap = new HeapOfMaximalSize<int>(size);
            RandomEnum rndE = new RandomEnum(randomSeed);
            List<int> inputs = rndE.Take(size).ToList();
            List<int> outputs = new List<int>();

            //act
            for (int i = 0; i < inputs.Count; i++)
                heap.Insert(inputs[i]);

            for (int i = 0; i < size; i++)
                outputs.Add(heap.ExtractMin());

            //assert
            var expected = inputs;
            expected.Sort();

            CollectionAssert.AreEqual(expected, outputs);
        }

    }

    class RandomEnum : IEnumerable<int>
    {
        public RandomEnum(int seed)
        {
            rnd = new Random(seed);
        }

        Random rnd;

        public IEnumerator<int> GetEnumerator()
        {
            while (true)
                yield return rnd.Next(0,100);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}