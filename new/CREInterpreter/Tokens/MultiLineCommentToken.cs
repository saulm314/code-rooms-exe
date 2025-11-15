using System;

namespace CREInterpreter.Tokens;

public class MultiLineCommentToken(ReadOnlyMemory<char> text, int lineNumber, int index) : IToken
{
    public ReadOnlyMemory<char> Text => text;

    public int LineNumber => lineNumber;

    public int Index => index;
}