namespace CREBlazorApp.Pages;

public static class DefaultStrings
{
    public static readonly string TextEditor = """
        int number = 5;
        double pi = 3.14;
        char character = 'a';
        bool condition = false;

        string[] animals = new string[] { "cat", "dog", "rabbit" };
        string dog = animals[1];
        animals = null;
        dog = null;

        string s = new string(new char[] { 'a', 'b' });

        int number2 = 0;
        while (true)
        {
            if (true)
            {
                for (int i = 0; i < 5; i = i + 1)
                {
                    number2 = number2 + 1;
                }
                break;
            }
        }
        """;
}