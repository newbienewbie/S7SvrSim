using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.Shared
{
    public static class MergeIEnumerableExtensions
    {
        /// <summary>
        /// 合并内部的迭代器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> values)
        {
            return new RecursiveEnumerable<T>(values);
        }

        /// <summary>
        /// 二级迭代器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class RecursiveEnumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerable<IEnumerable<T>> enumerable;

            public RecursiveEnumerable(IEnumerable<IEnumerable<T>> enumerable)
            {
                this.enumerable = enumerable;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new RecursiveEnumerator(enumerable);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private class RecursiveEnumerator : IEnumerator<T>
            {
                private readonly IEnumerator<IEnumerable<T>> rootEnumerator;
                private IEnumerator<T> currentEnumerator;

                public RecursiveEnumerator(IEnumerable<IEnumerable<T>> values)
                {
                    this.rootEnumerator = values.GetEnumerator();
                }

                public T Current
                {
                    get
                    {
                        // currentEnumerator 为 null 时，说明还没有开始迭代或者当前迭代的部分，对应的 IEnumerable 为 null 了
                        // 没有迭代时，rootEnumerator 的迭代器会返回迭代没有开始的报错
                        currentEnumerator ??= rootEnumerator.Current?.GetEnumerator();
                        // 因为自身在 MoveNext 时就会跳过空数组，所以这种还为 null 的情况不太可能出现。
                        // 但为了防止一手，如果还是为 null 了，就给一个空数组迭代，这样可以返回迭代已完成的错误
                        if (currentEnumerator == null)
                        {
                            currentEnumerator = System.Array.Empty<T>().Cast<T>().GetEnumerator();
                            currentEnumerator.MoveNext();
                        }
                        return currentEnumerator.Current;
                    }
                }

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                    rootEnumerator.Dispose();
                }

                /// <summary>
                /// 向后移动时自动跳过空容器
                /// </summary>
                /// <returns></returns>
                public bool MoveNext()
                {
                    if (currentEnumerator != null && currentEnumerator.MoveNext())
                    {
                        return true;
                    }

                    if (!rootEnumerator.MoveNext())
                    {
                        return false;
                    }

                    currentEnumerator = rootEnumerator.Current?.GetEnumerator();

                    while (currentEnumerator?.MoveNext() != true)
                    {
                        if (!rootEnumerator.MoveNext())
                        {
                            return false;
                        }

                        currentEnumerator = rootEnumerator.Current?.GetEnumerator();
                    }

                    return currentEnumerator != null;
                }

                public void Reset()
                {
                    rootEnumerator.Reset();
                    currentEnumerator = null;
                }
            }
        }
    }
}
