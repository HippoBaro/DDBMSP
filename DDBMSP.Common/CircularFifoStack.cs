﻿using System.Collections.Generic;

namespace DDBMSP.Common
{
    public class CircularFifoStack<T> : LinkedList<T>
    {
        private const int Size = 100;

        public void Push(T obj) {
            AddFirst(obj);
            
            while (Count > Size)
                RemoveLast();
        }

        public void Push(IEnumerable<T> obj) {
            foreach (var o in obj)
                AddFirst(o);

            while (Count > Size)
                RemoveLast();
        }
    }
}