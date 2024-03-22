using System;
using System.Collections;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Collections.Generic
{
    public interface IAltList<T1, T2> : IList
    {
        public Single<T1> Head { get; }
        public List<Pair<T2, T1>> Pairs { get; }
        public Single<T2> Tail { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T1 item in Head)
                yield return item;
            foreach (Pair<T2, T1> pair in Pairs)
                foreach (object item in pair)
                    yield return item;
            foreach (T2 item in Tail)
                yield return item;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            foreach (object value in this)
                array.SetValue(value, index++);
        }

        bool IList.Contains(object value)
        {
            foreach (object obj in this)
                if (obj == value)
                    return true;
            return false;
        }

        int IList.IndexOf(object value)
        {
            int i = 0;
            foreach (object obj in this)
            {
                if (obj == value)
                    return i;
                i++;
            }
            return -1;
        }
    }
}
