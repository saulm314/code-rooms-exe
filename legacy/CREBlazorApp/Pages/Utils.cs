namespace CREBlazorApp.Pages;

public static class Utils
{
    public static string GetSpaces(int n)
    {
        char[] chars = new char[n];
        Array.Fill(chars, ' ');
        return new(chars);
    }
}