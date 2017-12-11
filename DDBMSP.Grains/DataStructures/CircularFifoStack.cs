using System.Collections.Generic;

namespace DDBMSP.Grains.DataStructures
{
    public class CircularFifoStack<T> : Queue<T>
    {
        private const int Size = 100;

        public void Push(T obj)
        {
            Enqueue(obj);
            while (Count > Size)
            {
                Dequeue();
            }
        }
    }
}