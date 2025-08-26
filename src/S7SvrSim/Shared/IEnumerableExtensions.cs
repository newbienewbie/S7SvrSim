using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static void Swap<T>(this IList<T> list, T item1, T item2)
        {
            var index1 = list.IndexOf(item1);
            if (index1 < 0)
            {
                return;
            }
            var index2 = list.IndexOf(item2);
            if (index2 < 0)
            {
                return;
            }

            (list[index2], list[index1]) = (list[index1], list[index2]);
        }
    }
}
