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

        public new object this[int i]
        {
            get
            {
                if (IsIndexOutOfRange(i))
                    throw new ArgumentOutOfRangeException(i.ToString());
                GetSubIndexes(i, out int listIndex, out int pairIndex);
                if (listIndex == -1)
                    return Head[0];
                if (listIndex == -2)
                    return Tail[0];
                return Pairs[listIndex][pairIndex];
            }
            set
            {
                if (IsIndexOutOfRange(i))
                    throw new ArgumentOutOfRangeException(i.ToString());
                GetSubIndexes(i, out int listIndex, out int pairIndex);
                if (listIndex == -1)
                {
                    Head[0] = (T1)value;
                    return;
                }
                if (listIndex == -2)
                {
                    Tail[0] = (T2)value;
                    return;
                }
                Pair<T2, T1> pair = Pairs[listIndex];
                pair[pairIndex] = value;
            }
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

        public int IndexOfPair(Pair<T2, T1> pair)
        {
            int listIndex = Pairs.IndexOf(pair);
            if (listIndex == -1)
                return -1;
            return GetGlobalIndex(listIndex, 0);
        }

        public int IndexOfPair(Pair<T1, T2> pair)
        {
            int i = 0;
            bool currentlyT1;
            if (Head.Count == 1)
                currentlyT1 = true;
            currentlyT1 = false;
            Pair<T1, T2> currentPair = new(default, default);
            bool initialised = false;
            foreach (object item in this)
            {
                if (currentlyT1)
                {
                    currentlyT1 = false;
                    currentPair.First = (T1)item;
                    initialised = true;
                    i++;
                    continue;
                }
                currentlyT1 = true;
                currentPair.Second = (T2)item;
                if (initialised && pair.Equals(currentPair))
                    return --i;
                i++;
            }
            return -1;
        }

        public bool IsIndexOutOfRange(int index)
        {
            return index < 0 || index >= Count;
        }

        // return -1 if globalIndex points to head, or -2 if it points to tail
        public void GetSubIndexes(int globalIndex, out int listIndex, out int pairIndex)
        {
            if (globalIndex < Head.Count)
            {
                listIndex = pairIndex = -1;
                return;
            }
            if (Tail.Count > 0 && globalIndex > Count - Tail.Count - 1)
            {
                listIndex = pairIndex = -2;
                return;
            }
            listIndex = (globalIndex - Head.Count) / 2;
            pairIndex = (globalIndex - Head.Count) % 2;
        }

        public int GetGlobalIndex(int listIndex, int pairIndex)
        {
            if (listIndex < 0 || pairIndex < 0)
                return listIndex <= pairIndex ? listIndex : pairIndex;
            return listIndex * 2 + pairIndex + Head.Count;
        }
    }
}
