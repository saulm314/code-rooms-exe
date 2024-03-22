using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Collections.Generic
{
    public class AltListLockLock2<T1, T2> : IAltList<T1, T2>
    {
        public AltListLockLock2(T1 head, T2 tail)
        {
            Head = new(head);
            Tail = new(tail);
        }

        public Required<T1> Head { get; }
        public List<Pair<T2, T1>> Pairs { get; } = new();
        public Required<T2> Tail { get; }

        public IAltList<T1, T2> Base { get => _base ??= this; }
        private IAltList<T1, T2> _base;

        Single<T1> IAltList<T1, T2>.Head { get => Head; }
        Single<T2> IAltList<T1, T2>.Tail { get => Tail; }

        public int Count { get; private set; } = 2;
        public bool IsSynchronized { get; set; }
        public object SyncRoot { get; set; }
        public bool IsFixedSize { get; } = false;
        public bool IsReadOnly { get; } = false;

        public object this[int i] { get => this[i]; set => this[i] = value; }

        public int Add(object value) => Add((Pair<T1, T2>)value);
        public int Add(T1 t1, T2 t2) => Add(new(t1, t2));

        public int Add(Pair<T1, T2> pair)
        {
            Pairs.Add(new(default, default));
            Count += 2;
            this[Count - 3] = this[Count - 1];
            this[Count - 2] = pair.First;
            this[Count - 1] = pair.Second;
            return Count - 2;
        }

        public void Clear() => throw new NotSupportedException();

        public void Insert(int index, object value) => Insert(index, (Pair<T1, T2>)value);
        public void Insert(int index, T1 t1, T2 t2) => Insert(index, new(t1, t2));

        public void Insert(int index, Pair<T1, T2> pair)
        {
            if (Base.IsIndexOutOfRange(index) && index != Count)
                throw new ArgumentOutOfRangeException(index.ToString());
            Base.GetSubIndexes(index, out int listIndex, out int pairIndex);
            if (pairIndex == 0)
                throw new NotSupportedException("Cannot insert element in the middle of a pair");
            if (pairIndex == -2)
                throw new NotSupportedException("Cannot insert element into the list's tail");
            if (index == Count)
            {
                Add(pair);
                return;
            }

            // this happens to work correctly when listIndex is -1
            Pairs.Insert(listIndex + 1, new(default, default));
            Count += 2;
            this[index + 2] = this[index];
            this[index] = pair.First;
            this[index + 1] = pair.Second;
        }

        public void RemoveAt(int index)
        {
            if (Base.IsIndexOutOfRange(index))
                throw new ArgumentOutOfRangeException(index.ToString());
            if (index == Count - 1)
                throw new NotSupportedException("Cannot remove element from the list's tail");
            if (Count == 2)
                throw new NotSupportedException("Cannot remove the last element");
            Base.GetSubIndexes(index, out int listIndex, out int pairIndex);
            if (pairIndex == 0)
                throw new NotSupportedException("Cannot remove element in the middle of a pair");
            this[index] = this[index + 2];
            Pairs.RemoveAt(listIndex + 1);
            Count -= 2;
        }

        public void Remove(object value) => Remove((Pair<T1, T2>)value);
        public void Remove(T1 t1, T2 t2) => Remove(new(t1, t2));

        public void Remove(Pair<T1, T2> pair)
        {
            int index = Base.IndexOfPair(pair);
            if (index == -1)
                return;
            RemoveAt(index);
        }
    }
}
