using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Collections.Generic
{
    public class Required<T> : Single<T>
    {
        public Required(T? value) => Value = value;

        public T? Value { get; set; }

        public override IEnumerator<T?> GetEnumerator()
        {
            yield return Value;
        }

        public override int Count { get; } = 1;
        public override bool IsFixedSize { get; } = true;
        public override bool IsReadOnly { get; } = false;

        public override T? this[int i]
        {
            get
            {
                if (i != 0)
                    throw new ArgumentOutOfRangeException(i.ToString());
                return Value;
            }
            set
            {
                if (i != 0)
                    throw new ArgumentOutOfRangeException(i.ToString());
                Value = value;
            }
        }

        public override int Add(T? item) => throw new NotSupportedException();
        public override void Clear() => throw new NotSupportedException();
        public override bool Contains(T? item) => item?.Equals(Value) ?? Value == null;
        public override int IndexOf(T? item) => item?.Equals(Value) ?? Value == null ? 0 : -1;
        public override void Insert(int index, T? item) => throw new NotSupportedException();
        public override bool Remove(T? item) => throw new NotSupportedException();
        public override void RemoveAt(int index) => throw new NotSupportedException();
    }
}
