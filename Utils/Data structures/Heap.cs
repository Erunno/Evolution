using System;
using System.Text;

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

        /// <summary>
        /// Returns max element of heap, but dont remove it
        /// </summary>
        public T PeekMin() => Count != 0 ? heap[0] : throw new EmptyHeapExeption();

        /// <summary>
        /// Returns max element of heap and removes it
        /// </summary>
        public T ExtractMin()
        {
            if (Count == 0) throw new EmptyHeapExeption();

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
    }

    public class EmptyHeapExeption : Exception { }
    public class HeapOverflowException : Exception { }
}
