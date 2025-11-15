namespace CREBlazorApp.Pages;

public readonly record struct StackVarInfo(string SeparatorColor, VarInfo PVarInfo, string Name, int FontSize)
{
    public static int GetFontSize(int length)
        => length switch
        {
            <= 12 => 16,
            <= 13 => 14,
            <= 14 => 13,
            <= 16 => 12,
            <= 17 => 11,
            <= 19 => 10,
            <= 21 => 9,
            _ => 8
        };
}