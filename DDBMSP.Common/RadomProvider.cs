using System;

namespace DDBMSP.Common
{
    public static class RadomProvider
    {
        [ThreadStatic] private static Random _randm;
        public static Random Instance => _randm ?? (_randm = new Random());
    }
}