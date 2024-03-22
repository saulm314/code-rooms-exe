using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Collections.Generic
{
    public class Empty<T> : Single<T>, IReadOnlyList<T>
    {
        public override IEnumerator<T> GetEnumerator()
        {
            yield break;
        }

        public override int Count { get; } = 0;
        public override bool IsFixedSize { get; } = true;
        public override bool IsReadOnly { get; } = true;
        public override T this[int i]
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        public override int Add(T item) => throw new NotSupportedException();
        public override void Clear() { }
        public override bool Contains(T item) => false;
        public override int IndexOf(T item) => -1;
        public override void Insert(int index, T item) => throw new NotSupportedException();
        public override bool Remove(T item) => throw new NotSupportedException();
        public override void RemoveAt(int index) => throw new NotSupportedException();
    }
}
