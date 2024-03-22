using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Collections.Generic
{
    public class Optional<T> : Single<T>
    {
        private T _value;

        public override IEnumerator<T> GetEnumerator()
        {
            if (_count == 0)
                yield break;
            yield return _value;
        }

        public override int Count { get => _count; }
        private int _count = 0;

        public override bool IsFixedSize { get; } = false;
        public override bool IsReadOnly { get; } = false;

        public override T this[int i]
        {
            get
            {
                if (_count == 0)
                    throw new ArgumentOutOfRangeException(i.ToString());
                if (i != 0)
                    throw new ArgumentOutOfRangeException(i.ToString());
                return _value;
            }
            set
            {
                if (_count == 0)
                    throw new ArgumentOutOfRangeException(i.ToString());
                if (i != 0)
                    throw new ArgumentOutOfRangeException(i.ToString());
                _value = value;
            }
        }

        public override int Add(T item)
        {
            if (_count != 0)
                throw new NotSupportedException("Element already exists");
            if (_count == 0)
            {
                _count = 1;
                _value = item;
            }
            return 0;
        }

        public override void Clear()
        {
            _value = default;
            _count = 0;
        }

        public override bool Contains(T item)
        {
            if (_count == 0)
                return false;
            return item.Equals(_value);
        }

        public override int IndexOf(T item)
        {
            if (_count == 0)
                return -1;
            return item.Equals(_value) ? 0 : -1;
        }

        public override void Insert(int index, T item)
        {
            if (_count != 0)
                throw new NotSupportedException("Element already exists");
            if (index != 0)
                throw new ArgumentOutOfRangeException(index.ToString());
            _count = 1;
            _value = item;
        }

        public override bool Remove(T item)
        {
            if (_count == 0)
                return false;
            if (!item.Equals(_value))
                return false;
            _count = 0;
            _value = default;
            return true;
        }

        public override void RemoveAt(int index)
        {
            if (_count == 0)
                throw new ArgumentOutOfRangeException(index.ToString());
            if (index != 0)
                throw new ArgumentOutOfRangeException(index.ToString());
            _count = 0;
            _value = default;
        }
    }
}
