using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Collections.Generic
{
    public class Required<T> : Single<T>, IReadOnlyList<T>
    {
        public T Value { get; set; }

        public override IEnumerator<T> GetEnumerator()
        {
            yield return Value;
        }

        public override int Count { get; } = 1;
        public override bool IsFixedSize { get; } = true;
        public override bool IsReadOnly { get; } = true;
        public override T this[int i] { get => Value; set => Value = value; }
        public override int Add(T item) => throw new InvalidOperationException();
        public override void Clear() => throw new InvalidOperationException();
        public override bool Contains(T item) => item.Equals(Value);
        public override int IndexOf(T item) => item.Equals(Value) ? 0 : -1;
        public override void Insert(int index, T item) => throw new InvalidOperationException();
        public override bool Remove(T item) => throw new InvalidOperationException();
        public override void RemoveAt(int index) => throw new InvalidOperationException();
    }
}
