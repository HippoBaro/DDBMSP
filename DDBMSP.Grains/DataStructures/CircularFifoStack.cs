﻿using System.Collections.Generic;

namespace DDBMSP.Grains.DataStructures
{
    public class CircularFifoStack<T> : LinkedList<T>
    {
        private const int Size = 100;

        public void Push(T obj)
        {
            AddFirst(obj);
            while (Count > Size)
            {
                RemoveLast();
            }
        }
    }
}