namespace CRECSharpInterpreter
{
    public class StringConstruction
    {
        public StringConstruction(KeyString[] keyStringsInsideBrackets)
        {
            CharArrayExpression = new(keyStringsInsideBrackets);
            if (CharArrayExpression._VarType != VarType.@char.Array)
                throw new StringConstructionException(this, "char[] type expected in string constructor");
        }

        public int? CharArrayReference { get; set; }
        public Expression CharArrayExpression { get; init; }

        public class StringConstructionException : InterpreterException
        {
            public StringConstructionException(StringConstruction? stringConstruction, string? message = null) : base(message)
            {
                this.stringConstruction = stringConstruction;
            }

            public StringConstruction? stringConstruction;
        }
    }
}
