using System.Collections;
using System.Collections.Generic;

namespace CRECSharpInterpreter
{
    public class Heap : IEnumerable<Variable?>
    {
        public static int initialHeapCapacity = 50;
        private static int heapAdditionCapacity = initialHeapCapacity;

        public Heap()
        {
            variables.AddRange(GetNullVariables(initialHeapCapacity));
            variables[0] = new(null);
        }

        internal Heap(int capacity)
        {
            variables.AddRange(GetNullVariables(capacity));
            variables[0] = new(null);
        }

        private IEnumerable<Variable?> GetNullVariables(int count)
        {
            for (int i = 0; i < count; i++)
                yield return null;
        }

        public int Size { get; internal set; } = initialHeapCapacity;

        internal List<Variable?> variables = new(initialHeapCapacity);
        public Variable? this[int i] { get => variables[i]; internal set => variables[i] = value; }

        public IEnumerator<Variable?> GetEnumerator()
        {
            for (int i = 0; i < Size; i++)
                yield return variables[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Size; i++)
                yield return variables[i];
        }

        public int Allocate(int length, IEnumerable<Variable> data)
        {
            HeapLengthVariable lengthAsVariable = new();
            lengthAsVariable.Value = length;
            int sizeRequired = length + 1;
            int index = GetEmptySpace(sizeRequired);
            variables[index] = lengthAsVariable;
            int i = index;
            foreach (Variable variable in data)
            {
                variables[++i] = variable;
                if (i >= index + length)
                    break;
            }
            return index;
        }

        // we assume that there are no circular references
        public void Free(int index)
        {
            Variable lengthAsVariable = variables[index]!;
            int length = (int)lengthAsVariable.Value!;
            for (int i = index; i <= index + length; i++)
            {
                bool isNonNullReferenceVariable =
                    variables[i]!._VarType!._Storage == VarType.Storage.Reference &&
                    variables[i]!.Value != null;
                if (isNonNullReferenceVariable)
                    DecrementReferenceCounter((int)variables[i]!.Value!);
                variables[i] = null;
            }
        }

        public object? GetValue(int index, int offset)
        {
            VerifyRange(index, offset);
            Variable variable = variables[index + offset + 1]!;
            return variable.Value;
        }

        public string GetValueAsString(int index, int offset)
        {
            VerifyRange(index, offset);
            Variable variable = variables[index + offset + 1]!;
            return variable.ValueAsString;
        }

        public void SetValue(int index, int offset, object? value)
        {
            VerifyRange(index, offset);
            Variable variable = variables[index + offset + 1]!;
            variable.Value = value;
        }

        public int GetLength(int index)
        {
            Variable lengthVariable = variables[index]!;
            return (int)lengthVariable.Value!;
        }

        public void IncrementReferenceCounter(int index)
        {
            HeapLengthVariable lengthVariable = (HeapLengthVariable)variables[index]!;
            lengthVariable.referenceCount++;
        }

        public void DecrementReferenceCounter(int index)
        {
            HeapLengthVariable lengthVariable = (HeapLengthVariable)variables[index]!;
            if (--lengthVariable.referenceCount == 0)
                Free(index);
        }

        public T[]? GetArray<T>(int index)
        {
            if (index == 0)
                return null;
            T[] array = new T[GetLength(index)];
            for (int i = 0; i < array.Length; i++)
                array[i] = (T)GetValue(index, i)!;
            return array;
        }

        public string? GetString(int index)
        {
            if (index == 0)
                return null;
            char[] chars = GetArray<char>(index)!;
            string str = new(chars);
            return str;
        }

        public string?[]? GetStringArray(int index)
        {
            if (index == 0)
                return null;
            int[] stringIndexes = GetArray<int>(index)!;
            string?[] strings = new string?[stringIndexes.Length];
            for (int i = 0; i < stringIndexes.Length; i++)
                strings[i] = GetString(index);
            return strings;
        }

        private void VerifyRange(int index, int offset)
        {
            Variable lengthAsVariable = variables[index]!;
            if (lengthAsVariable._VarType != VarType.@int)
                throw new HeapException(this, $"Internal exception: expecting int (length) at index {index}");
            if (lengthAsVariable.Value == null)
                throw new HeapException(this, $"Internal exception: length value cannot be null");
            int length = (int)lengthAsVariable.Value;
            if (offset >= length || offset < 0)
                throw new HeapException(this, "Cannot access element out of range");
        }

        // return index of beginning of empty space where a data structure of a certain size will fit
        private int GetEmptySpace(int size)
        {
            for (int i = 0; i < variables.Count; i++)
                if (IsSpaceEmpty(i, size))
                    return i;
            ResizeHeap();
            return GetEmptySpace(size);
        }

        private bool IsSpaceEmpty(int index, int size)
        {
            if (index + size > variables.Count)
                return false;
            for (int i = index; i < index + size; i++)
                if (variables[i] != null)
                    return false;
            return true;
        }

        // returns index of where new heap begins
        private int ResizeHeap()
        {
            int index = variables.Capacity;
            variables.Capacity += heapAdditionCapacity;
            variables.AddRange(GetNullVariables(heapAdditionCapacity));
            Size = variables.Capacity;
            return index;
        }

        public class HeapException : InterpreterException
        {
            public HeapException(Heap? heap, string? message = null) : base(message)
            {
                this.heap = heap;
            }

            public Heap? heap;
        }
    }
}
