﻿using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Collections.Generic
{
    public class AltListLockLock1<T1, T2> : IAltList<T1, T2>
    {
        public AltListLockLock1(T1? head) => Head = new(head);

        public Required<T1> Head { get; }
        public List<Pair<T2, T1>> Pairs { get; } = new();
        public Empty<T2> Tail { get; } = new();

        public IAltList<T1, T2> Base { get => _base ??= this; }
        private IAltList<T1, T2>? _base;

        Single<T1> IAltList<T1, T2>.Head { get => Head; }
        Single<T2> IAltList<T1, T2>.Tail { get => Tail; }

        public int Count { get; private set; } = 1;
        public bool IsFixedSize { get; } = false;
        public bool IsReadOnly { get; } = false;

        public object? this[int i] { get => Base.Get(i); set => Base.Set(i, value); }

        public int Add(object? value) => value is not null ? Add((Pair<T2, T1>)value) : -1;
        public int Add(T2? t2, T1? t1) => Add(new(t2, t1));

        public int Add(Pair<T2, T1> pair)
        {
            Pairs.Add(pair);
            Count += 2;
            return Count - 2;
        }

        public void Clear() => throw new NotSupportedException();

        public void Insert(int index, object? value) => Insert(index, (Pair<T2, T1>)value!);
        public void Insert(int index, T2? t2, T1? t1) => Insert(index, new(t2, t1));

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

        public void Remove(object? value) => Remove((Pair<T2, T1>)value!);
        public void Remove(T2? t2, T1? t1) => Remove(new(t2, t1));

        public void Remove(Pair<T2, T1> pair)
        {
            int index = Base.IndexOfPair(pair);
            if (index == -1)
                return;
            RemoveAt(index);
        }

        public AltListLockLock1<T1, T2> Sublist(int startIndex, int endIndex)
        {
            if (startIndex % 2 == 1)
                throw new NotSupportedException("Cannot create sublist from an odd index");
            if (endIndex % 2 == 0)
                throw new NotSupportedException("Cannot create sublist stopping at an even index");
            if (Base.IsIndexOutOfRange(startIndex))
                throw new ArgumentOutOfRangeException(startIndex.ToString());
            if (Base.IsIndexOutOfRange(endIndex) && endIndex != Count)
                throw new ArgumentOutOfRangeException(endIndex.ToString());
            AltListLockLock1<T1, T2> sublist = new((T1?)this[startIndex]);
            Pair<T2, T1> currentPair = new(default, default);
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                switch (i % 2)
                {
                    case 1:
                        currentPair[0] = this[i];
                        break;
                    case 0:
                        currentPair[1] = this[i];
                        sublist.Add(currentPair);
                        currentPair = new(default, default);
                        break;
                }
            }
            return sublist;
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach (object? obj in this)
                str += (obj?.ToString() ?? "null") + "\n";
            return str;
        }
    }
}
