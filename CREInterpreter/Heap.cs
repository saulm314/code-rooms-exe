using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CREInterpreter;

public class Heap(int initialCapacity) : IReadOnlyList<Variable?>
{
    public ImmutableList<VariableContainer> VariableContainers { get; private set; } =
        ImmutableList<VariableContainer>.Empty
        .AddRange(BlankEnumerables.GetBlankVariableContainers(initialCapacity));

    public int Allocate(int length, IEnumerable<Variable> data)
    {
        HeapLengthVariable lengthVariable = new()
        {
            Value = length
        };
        int sizeRequired = length + 1;
        int index = GetEmptySpace(sizeRequired);
        VariableContainers[index]._Variable = lengthVariable;
        int i = index;
        foreach (Variable variable in data)
        {
            VariableContainers[++i]._Variable = variable;
            if (i >= index + length)
                break;
        }
        return index;
    }

    // we assume that there are no circular references
    public void Free(int index)
    {
        Variable lengthVariable = VariableContainers[index]._Variable!;
        int length = (int)lengthVariable.Value!;
        for (int i = index; i <= index + length; i++)
        {
            bool isNonNullReferenceVariable =
                VariableContainers[i]._Variable!._VarType!._Storage == Storage.Reference &&
                VariableContainers[i]._Variable!.Value != null;
            if (isNonNullReferenceVariable)
                DecrementReferenceCounter((int)VariableContainers[i]._Variable!.Value!);
            VariableContainers[i]._Variable = null;
        }
    }

    public object? GetValue(int index, int offset)
    {
        VerifyRange(index, offset);
        Variable variable = VariableContainers[index + offset + 1]._Variable!;
        return variable.Value;
    }

    public string GetFormattedValue(int index, int offset)
    {
        VerifyRange(index, offset);
        Variable variable = VariableContainers[index + offset + 1]._Variable!;
        return variable.FormattedValue;
    }

    public void SetValue(int index, int offset, object? value)
    {
        VerifyRange(index, offset);
        Variable variable = VariableContainers[index + offset + 1]._Variable!;
        variable.Value = value;
    }

    public int GetLength(int index)
    {
        Variable lengthVariable = VariableContainers[index]._Variable!;
        return (int)lengthVariable.Value!;
    }

    public void IncrementReferenceCounter(int index)
    {
        HeapLengthVariable lengthVariable = (HeapLengthVariable)VariableContainers[index]._Variable!;
        lengthVariable.ReferenceCount++;
    }

    public void DecrementReferenceCounter(int index)
    {
        HeapLengthVariable lengthVariable = (HeapLengthVariable)VariableContainers[index]._Variable!;
        if (--lengthVariable.ReferenceCount == 0)
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
        Variable lengthVariable = VariableContainers[index]._Variable!;
        if (lengthVariable._VarType != VarType.@int)
            throw new InterpreterException($"Internal error: expecting int variable at heap index {index}");
        if (lengthVariable.Value == null)
            throw new InterpreterException($"Internal error: length value cannot be null");
        int length = (int)lengthVariable.Value;
        if (offset >= length || offset < 0)
            throw new InterpreterException("Cannot access element out of range");
    }

    // return index of beginning of empty space where a data structure of a certain size will fit
    private int GetEmptySpace(int size)
    {
        for (int i = 0; i < VariableContainers.Count; i++)
            if (IsSpaceEmpty(i, size))
                return i;
        ResizeHeap();
        return GetEmptySpace(size);
    }

    private bool IsSpaceEmpty(int index, int size)
    {
        if (index + size > VariableContainers.Count)
            return false;
        for (int i = index; i < index + size; i++)
            if (VariableContainers[i]._Variable != null)
                return false;
        return true;
    }

    // returns index of where new heap begins
    private int ResizeHeap()
    {
        int index = VariableContainers.Count;
        VariableContainers = VariableContainers.AddRange(BlankEnumerables.GetBlankVariableContainers(initialCapacity));
        return index;
    }

    public Variable? this[int i] => VariableContainers[i]._Variable;

    public int Count => VariableContainers.Count;

    public IEnumerator<Variable?> GetEnumerator()
    {
        foreach (VariableContainer variableContainer in VariableContainers)
            yield return variableContainer._Variable;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (VariableContainer variableContainer in VariableContainers)
            yield return variableContainer._Variable;
    }
}