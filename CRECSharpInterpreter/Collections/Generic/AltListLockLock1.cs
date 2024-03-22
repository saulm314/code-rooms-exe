using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Collections.Generic
{
    public class AltListLockLock1<T1, T2> : IAltList<T1, T2>
    {
        public AltListLockLock1(T1 head) => Head = new(head);

        public Required<T1> Head { get; }
        public List<Pair<T2, T1>> Pairs { get; } = new();
        public Empty<T2> Tail { get; } = new();

        public IAltList<T1, T2> Base { get => _base ??= this; }
        private IAltList<T1, T2> _base;

        Single<T1> IAltList<T1, T2>.Head { get => Head; }
        Single<T2> IAltList<T1, T2>.Tail { get => Tail; }

        public int Count { get; private set; } = 1;
        public bool IsSynchronized { get; set; }
        public object SyncRoot { get; set; }
        public bool IsFixedSize { get; } = false;
        public bool IsReadOnly { get; } = false;

        public int Add(object value) => Add((Pair<T2, T1>)value);
        public int Add(T2 t2, T1 t1) => Add(new(t2, t1));

        public int Add(Pair<T2, T1> pair)
        {
            Pairs.Add(pair);
            Count += 2;
            return Count - 2;
        }

        public void Clear() => throw new NotSupportedException();

        public void Insert(int index, object value) => Insert(index, (Pair<T2, T1>)value);
        public void Insert(int index, T2 t2, T1 t1) => Insert(index, new(t2, t1));

        public void Insert(int index, Pair<T2, T1> pair)
        {
            if (Base.IsIndexOutOfRange(index) && index != Count)
                throw new ArgumentOutOfRangeException(index.ToString());
            if (index == 0)
                throw new NotSupportedException("Cannot insert element into the list's head");
            Base.GetSubIndexes(index, out int listIndex, out int pairIndex);
            if (pairIndex != 0)
                throw new NotSupportedException("Cannot insert element in the middle of a pair");
            Pairs.Insert(listIndex, pair);
            Count += 2;
        }

        public void RemoveAt(int index)
        {
            if (Base.IsIndexOutOfRange(index))
                throw new ArgumentOutOfRangeException(index.ToString());
            if (index == 0)
                throw new NotSupportedException("Cannot remove element from the list's head");
            Base.GetSubIndexes(index, out int listIndex, out int pairIndex);
            if (pairIndex != 0)
                throw new NotSupportedException("Cannot remove element in the middle of a pair");
            Pairs.RemoveAt(listIndex);
            Count -= 2;
        }

        public void Remove(object value) => Remove((Pair<T2, T1>)value);
        public void Remove(T2 t2, T1 t1) => Remove(new(t2, t1));

        public void Remove(Pair<T2, T1> pair)
        {
            int index = Base.IndexOfPair(pair);
            if (index == -1)
                return;
            RemoveAt(index);
        }
    }
}
