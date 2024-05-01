using System;
using System.Linq;
using RealVariable = CRECSharpInterpreter.Variable;

namespace CRECSharpInterpreter.Levels.Tests
{
    public class ReferenceTypes : ILevelTest
    {
        public int StarsAchieved(int cycle)
        {
            int count = 0;
            if (Star1(cycle))
                count++;
            if (Star2(cycle))
                count++;
            if (Star3(cycle))
                count++;
            if (Star4(cycle))
                count++;
            return count;
        }

        private bool Star1(int cycle)
        {
            return
                (int?)Memory.Instance!.Heap[1]?.Value == 4 &&
                Memory.Instance.Heap[2]?._VarType == VarType.@bool &&
                Memory.Instance.Heap[3]?._VarType == VarType.@bool &&
                Memory.Instance.Heap[4]?._VarType == VarType.@bool &&
                Memory.Instance.Heap[5]?._VarType == VarType.@bool;
        }

        private bool Star2(int cycle)
        {
            return
                (int?)Memory.Instance!.Heap[1]?.Value == 4 &&
                Memory.Instance.Heap[2]?._VarType == VarType.@bool &&
                Memory.Instance.Heap[3]?._VarType == VarType.@bool &&
                Memory.Instance.Heap[4]?._VarType == VarType.@bool &&
                (bool?)Memory.Instance.Heap[5]?.Value == true;
        }

        private bool Star3(int cycle)
        {
            return
                (int?)Memory.Instance!.Heap[1]?.Value == 4 &&
                Memory.Instance.Heap[2]?._VarType == VarType.@bool &&
                Memory.Instance.Heap[3]?._VarType == VarType.@bool &&
                Memory.Instance.Heap[4]?._VarType == VarType.@bool &&
                (bool?)Memory.Instance.Heap[5]?.Value == true &&
                (int?)Memory.Instance.Heap[6]?.Value == 3 &&
                (char?)Memory.Instance.Heap[7]?.Value == 'x' &&
                (char?)Memory.Instance.Heap[8]?.Value == 'y' &&
                (char?)Memory.Instance.Heap[9]?.Value == 'z' &&
                Array.FindAll(Memory.Instance.GetDeclaredVariables().ToArray(), array => array?._VarType == VarType.@char.Array
                    && (int?)array?.Value == 6).Length >= 2;
        }

        private bool Star4(int cycle)
        {
            return
                (int?)Memory.Instance!.Heap[1]?.Value == 4 &&
                Memory.Instance.Heap[2]?._VarType == VarType.@bool &&
                Memory.Instance.Heap[3]?._VarType == VarType.@bool &&
                Memory.Instance.Heap[4]?._VarType == VarType.@bool &&
                (bool?)Memory.Instance.Heap[5]?.Value == true &&
                (int?)Memory.Instance.Heap[6]?.Value == 3 &&
                (char?)Memory.Instance.Heap[7]?.Value == 'x' &&
                (char?)Memory.Instance.Heap[8]?.Value == 'y' &&
                (char?)Memory.Instance.Heap[9]?.Value == 'z' &&
                Array.FindAll(Memory.Instance.GetDeclaredVariables().ToArray(), array => array?._VarType == VarType.@char.Array
                    && (int?)array?.Value == 6).Length >= 2 &&
                (int?)Memory.Instance.Heap[10]?.Value == 3 &&
                (char?)Memory.Instance.Heap[11]?.Value == 'x' &&
                (char?)Memory.Instance.Heap[12]?.Value == 'y' &&
                (char?)Memory.Instance.Heap[13]?.Value == 'z';
        }
    }
}
