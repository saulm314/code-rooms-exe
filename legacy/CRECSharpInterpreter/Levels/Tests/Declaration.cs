using RealVariable = CRECSharpInterpreter.Variable;

namespace CRECSharpInterpreter.Levels.Tests
{
    public class Declaration : ILevelTest
    {
        public int StarsAchieved(int cycle)
        {
            (bool, bool, bool, bool) typeDeclared = (false, false, false, false);
            foreach (RealVariable variable in Memory.Instance!.GetDeclaredVariables())
            {
                switch (variable._VarType!.Slug)
                {
                    case "int":
                        typeDeclared.Item1 = true;
                        break;
                    case "double":
                        typeDeclared.Item2 = true;
                        break;
                    case "char":
                        typeDeclared.Item3 = true;
                        break;
                    case "bool":
                        typeDeclared.Item4 = true;
                        break;
                }
            }
            return typeDeclared == (true, true, true, true) ? 1 : 0;
        }
    }
}
