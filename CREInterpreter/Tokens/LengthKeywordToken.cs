﻿namespace CREInterpreter.Tokens;

public class LengthKeywordToken(int lineNumber) : IToken, IKeyword
{
    public string Text => "Length";

    public int LineNumber => lineNumber;
}