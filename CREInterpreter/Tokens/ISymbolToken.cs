namespace CREInterpreter.Tokens;

public interface ISymbolToken : IToken
{
    new string Text { get; }
}