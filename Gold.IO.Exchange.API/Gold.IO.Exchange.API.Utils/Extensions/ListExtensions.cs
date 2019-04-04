using System;
using System.Collections.Generic;

namespace Gold.IO.Exchange.API.Utils.Extensions
{
    public static class ListExtensions
    {
        public static void ForEachNext<T>(this List<T> collection, Action<T, T> func)
        {
            for (int i = 0; i < collection.Count - 1; i++)
                func(collection[i], collection[i + 1]);
        }
    }
}
