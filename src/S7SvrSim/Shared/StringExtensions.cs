using System.Collections;
using System.Collections.Generic;

namespace S7SvrSim.Shared
{
    internal static class StringExtensions
    {
        private class StringWrappedEnumerable : IEnumerable<string>
        {
            private readonly string input;
            private readonly string wrap;
            private readonly bool skipEmpty;

            public StringWrappedEnumerable(string input, string wrap, bool skipEmpty)
            {
                this.input = input;
                this.wrap = wrap;
                this.skipEmpty = skipEmpty;
            }

            private class StringWrappedEnumertor : IEnumerator<string>
            {
                private readonly string input;
                private readonly string wrap;
                private readonly bool skipEmpty;
                private string _current = null!;
                private int _preWrapIndex;
                private int _secWrapIndex;

                public string Current => _current;

                object IEnumerator.Current => Current;

                public StringWrappedEnumertor(string input, string wrap, bool skipEmpty)
                {
                    this.input = input;
                    this.wrap = wrap;
                    this.skipEmpty = skipEmpty;
                    Reset();
                }

                public void Dispose()
                {

                }

                public bool MoveNext()
                {
                    _preWrapIndex = _secWrapIndex == -1 ? input.IndexOf(wrap) : input.IndexOf(wrap, _secWrapIndex + 1);
                    if (_preWrapIndex == -1) return false;

                    _secWrapIndex = input.IndexOf(wrap, _preWrapIndex + 1);
                    if (_secWrapIndex == -1) return false;

                    var len = _secWrapIndex - _preWrapIndex - 1;
                    if (len == 0 && skipEmpty) return MoveNext();

                    _current = input.Substring(_preWrapIndex + 1, len);
                    return true;
                }

                public void Reset()
                {
                    _current = input;
                    _preWrapIndex = -1;
                    _secWrapIndex = -1;
                }
            }

            public IEnumerator<string> GetEnumerator()
            {
                return new StringWrappedEnumertor(input, wrap, skipEmpty);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static IEnumerable<string> FindWrapped(this string input, string wrap, bool skipEmpty = true)
        {
            return new StringWrappedEnumerable(input, wrap, skipEmpty);
        }
    }
}
