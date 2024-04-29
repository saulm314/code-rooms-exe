using RealVariable = CRECSharpInterpreter.Variable;

namespace CRECSharpInterpreter.Levels.Tests
{
    public class ReverseArray : ILevelTest
    {
        public int StarsAchieved(int cycle)
        {
            switch (cycle)
            {
                case 0:
                    bool success = 
                        (int)Memory.Instance!.Heap[1]!.Value! == 3 &&
                        (char)Memory.Instance!.Heap[2]!.Value! == 't' &&
                        (char)Memory.Instance!.Heap[3]!.Value! == 'a' &&
                        (char)Memory.Instance!.Heap[4]!.Value! == 'c';
                    if (!success)
                        return 0;
                    if (IsElement16Assigned())
                        return 1;
                    if (IsElement8Assigned())
                        return 2;
                    return 3;
                case 1:
                    bool success1 =
                        (int)Memory.Instance!.Heap[1]!.Value! == 6 &&
                        (char)Memory.Instance!.Heap[2]!.Value! == 't' &&
                        (char)Memory.Instance!.Heap[3]!.Value! == 'i' &&
                        (char)Memory.Instance!.Heap[4]!.Value! == 'b' &&
                        (char)Memory.Instance!.Heap[5]!.Value! == 'b' &&
                        (char)Memory.Instance!.Heap[6]!.Value! == 'a' &&
                        (char)Memory.Instance!.Heap[7]!.Value! == 'r';
                    if (!success1)
                        return 0;
                    if (IsElement16Assigned())
                        return 1;
                    if (IsElement8Assigned())
                        return 2;
                    return 3;
                case 2:
                    bool success2 = 
                        (int)Memory.Instance!.Heap[1]!.Value! == 3 &&
                        (char)Memory.Instance!.Heap[2]!.Value! == 'g' &&
                        (char)Memory.Instance!.Heap[3]!.Value! == 'o' &&
                        (char)Memory.Instance!.Heap[4]!.Value! == 'd';
                    if (!success2)
                        return 0;
                    if (IsElement16Assigned())
                        return 1;
                    if (IsElement8Assigned())
                        return 2;
                    return 3;
            }
            return 0;
        }

        private bool IsElement8Assigned()
        {
            foreach (MemoryFrame frame in Memory.Instance!.Frames)
                if (frame.Heap![8] != null)
                    return true;
            return false;
        }

        private bool IsElement16Assigned()
        {
            foreach (MemoryFrame frame in Memory.Instance!.Frames)
                if (frame.Heap![16] != null)
                    return true;
            return false;
        }
    }
}
