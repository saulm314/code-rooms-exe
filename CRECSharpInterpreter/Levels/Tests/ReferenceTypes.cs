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
                (char?)Memory.Instance.Heap[9]?.Value == 'z';
        }
    }
}
