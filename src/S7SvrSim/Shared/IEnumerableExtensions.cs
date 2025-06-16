using System;
using System.Collections.Generic;

namespace S7SvrSim.Shared
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> objects, Action<T> action)
        {
            foreach (var item in objects)
            {
                action?.Invoke(item);
            }
            return objects;
        }
    }
}
