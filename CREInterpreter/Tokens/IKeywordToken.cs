namespace CREInterpreter.Tokens;

public interface IKeywordToken : IToken
{
    new string Text { get; }
}