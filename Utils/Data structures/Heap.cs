using System;
using System.Text;
using System.Collections.Generic;

namespace Utils
{
    public class HeapOfMaximalSize<T> where T : IComparable<T>
    {
        public HeapOfMaximalSize(int size)
        {
            heap = new T[size];
            Count = 0;
        }

        private T[] heap;
        public int Count { get; private set; }
        private int Version { get; set; }

        /// <summary>
        /// Returns max element of heap, but dont remove it
        /// </summary>
        public T PeekMin() => Count != 0 ? heap[0] : throw new EmptyHeapExeption();

        public bool IsFull => Count == heap.Length;

        /// <summary>
        /// Returns max element of heap and removes it
        /// </summary>
        public T ExtractMin()
        {
            if (Count == 0) throw new EmptyHeapExeption();
            Version++;

            T itemToReturn = heap[0];
            heap[0] = heap[--Count];

            SinkElement(root: 0);
            return itemToReturn;
        }

        private void SinkElement(int root)
        {
            int left = LeftSon(root);
            int right = RightSon(root);
            int maximal = -1;

            if (left < Count && heap[left].CompareTo(heap[root]) < 0)
                maximal = left;
            if (right < Count && heap[right].CompareTo(heap[root]) < 0)
                if(heap[left].CompareTo(heap[right]) > 0)
                    maximal = right;

            if (maximal == -1) //if maximal hasnt been changed
                return;         //root is properly emplaced

            heap.Swap(maximal, root);
            SinkElement(maximal);
        }

        /// <summary>
        /// Emplace item to heap
        /// </summary>
        public void Insert(T item)
        {
            if (heap.Length == Count) throw new HeapOverflowException();

            Version++;

            heap[Count++] = item;
            ElevateLastMember();
        }

        private void ElevateLastMember()
        {
            int currNode = Count - 1;

            while(currNode > 0)
            {
                int parent = Parent(currNode);
                if (heap[parent].CompareTo(heap[currNode]) < 0)
                    return;

                heap.Swap(parent, currNode);
                currNode = parent;    
            }
        }


        /// <summary>
        /// Copies content of heap to given array
        /// </summary>
        public void CopyHeapToArray(T[] copy)
        {
            if (copy.Length < Count)
                throw new SmallArrayException();

            heap.CopyTo(copy, index: 0);
        }

        private int Parent(int node) => (node - 1) / 2;
        private int LeftSon(int node) => 2 * node + 1;
        private int RightSon(int node) => 2 * node + 2;

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < Count; i++)
                output.Append(heap[i].ToString() + " ");

            return output.ToString();
        }

        /// <summary>
        /// Creates new array and copies all elements to new array
        /// </summary>
        public void SetNewSize(int newSize)
        {
            if (newSize < Count)
                throw new UvalidSizeOfHeapException();

            T[] newHeap = new T[newSize];

            for (int i = 0; i < Count; i++)
                newHeap[i] = heap[i];

            heap = newHeap;
        }
    }

    public class EmptyHeapExeption : Exception { }
    public class HeapOverflowException : Exception { }
    public class SmallArrayException : Exception { }
    public class UvalidSizeOfHeapException : Exception { }
}
