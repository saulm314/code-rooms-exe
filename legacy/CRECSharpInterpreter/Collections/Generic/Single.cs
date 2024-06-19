using System;
using System.Collections;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Collections.Generic
{
    public abstract class Single<T> : IList<T?>, IList
    {
        public bool IsSynchronized { get => throw new NotSupportedException(); }
        public object SyncRoot { get => throw new NotSupportedException(); }

        public void CopyTo(Array array, int index)
        {
            foreach (T? value in this)
                array.SetValue(value, index++);
        }

        public void CopyTo(T?[] array, int index)
        {
            foreach (T? value in this)
                array[index++] = value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        object? IList.this[int i] { get => this[i]; set => this[i] = (T?)value; }
        public int Add(object? value) => Add((T?)value);
        void ICollection<T?>.Add(T? item) => Add(item);
        public bool Contains(object? value) => Contains((T?)value);
        public int IndexOf(object? value) => IndexOf((T?)value);
        public void Insert(int index, object? value) => Insert(index, (T?)value);
        public void Remove(object? value) => Remove((T?)value);

        // abstract members
        public abstract IEnumerator<T?> GetEnumerator();
        public abstract int Count { get; }
        public abstract bool IsFixedSize { get; }
        public abstract bool IsReadOnly { get; }
        public abstract T? this[int i] { get; set; }
        public abstract int Add(T? item);
        public abstract void Clear();
        public abstract bool Contains(T? item);
        public abstract int IndexOf(T? item);
        public abstract void Insert(int index, T? item);
        public abstract bool Remove(T? item);
        public abstract void RemoveAt(int index);
    }
}
