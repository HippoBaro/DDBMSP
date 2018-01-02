using System;
using System.Collections.Generic;

namespace DDBMSP.Common
{
    public class OrderedList<TValue> : List<TValue>
    {
        public void Trim(int maxItem) {
            RemoveRange(100, int.MaxValue);
        }
    }
}