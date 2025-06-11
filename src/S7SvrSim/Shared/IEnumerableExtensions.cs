using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.Shared
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> values)
        {
            IEnumerable<T> result = values.FirstOrDefault();
            foreach (var item in values.Skip(1))
            {
                result = result.Concat(item);
            }
            return result ?? [];
        }
    }
}
