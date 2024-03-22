﻿using System;
using System.Collections;

namespace CRECSharpInterpreter.Collections.Generic
{
    public class Pair<T1, T2> : IList
    {
        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public T1 First { get; set; }
        public T2 Second { get; set; }

        public IEnumerator GetEnumerator()
        {
            yield return First;
            yield return Second;
        }

        public int Count { get; } = 2;
        public bool IsSynchronized { get; set; }
        public object SyncRoot { get; set; }

        public void CopyTo(Array array, int index)
        {
            foreach (object value in this)
                array.SetValue(value, index++);
        }

        public bool IsFixedSize { get; } = true;
        public bool IsReadOnly { get; } = false;

        public object this[int i]
        {
            get
            {
                if (i == 0)
                    return First;
                if (i == 1)
                    return Second;
                throw new ArgumentOutOfRangeException(i.ToString());
            }
            set
            {
                if (i == 0)
                    First = (T1)value;
                if (i == 1)
                    Second = (T2)value;
                throw new ArgumentOutOfRangeException(i.ToString());
            }
        }

        public int Add(object value) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();

        public bool Contains(object value)
        {
            if (value == (object)First)
                return true;
            if (value == (object)Second)
                return true;
            return false;
        }

        public int IndexOf(object value)
        {
            if (value == (object)First)
                return 0;
            if (value == (object)Second)
                return 1;
            return -1;
        }

        public void Insert(int index, object value) => throw new NotSupportedException();
        public void Remove(object value) => throw new NotSupportedException();
        public void RemoveAt(int index) => throw new NotSupportedException();
    }
}
