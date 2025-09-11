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

    public static readonly string Description = """
        Welcome to CodeRooms.exe!

        This is a visualiser for how C#/Java memory works at runtime. It executes code statement by statement and shows you exactly what is going on in the stack and in the heap at each stage, including memory allocation, garbage collection, etc. You can also use the left and right arrows to go back to a previous point in the code's execution and then forward again.

        Try it out with the default code on the left. Hit "Compile", then hit "Run", and tap ">" repeatedly and observe what happens each time.

        Since there is a fully functioning interpreter, you can hit "Edit" and type any arbitrary C# code that you like, and then see what happens with each statement. (NOTE: if you see an error when you hit "Compile", that means your code has a compile-time error; this does not mean that the CodeRooms.exe app has a bug! For example, try creating a compile-time error on purpose and see what happens.)

        Note that only basic C# features are supported:
        - only procedural code - no methods or classes;
        - only basic data types: int, double, char, bool, string, array of each
        - unlike C#, strings are compared by reference - this is a design choice since the aim is to make value types and reference types intuitive for beginners, and the overloaded == operator will only confuse them
        - unlike C#, converting an int to a double requires an explicit cast - this is a design choice to make the beginner aware of different data types
        - array creation: either "new int[] { 5, 3, 2 }" or "new int[5]"
        - string creation: either a string literal or "new string(chars)" where chars is a char[]
        - if and if-else statements
        - while loops, for loops, break, continue
        - a few others...

        The buttons below are currently greyed out, because the frontend for them hasn't been implemented yet. This was originally made as an Avalonia desktop app, and just now ported to Blazor WebAssembly, but some features from the original Avalonia app are still in development. I expect to finish them in the next couple of days. In the meantime, you can find the original Avalonia app on GitHub: github.com/saulm314/code-rooms-exe or see a video here: https://youtu.be/sjaKQUeUtJM .
        """;
}