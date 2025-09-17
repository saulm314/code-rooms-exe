using CREBlazorApp.GeneratedCode;
using CRECSharpInterpreter.Levels;
using System.Net.Http.Json;

namespace CREBlazorApp.Pages;

public static class LevelManagerHandler
{
    public const string LevelDirectory = "Files/Levels/";

    public static async Task<LevelManager> NewLevelManager(HttpClient http)
    {
        Level[] levels = new Level[GeneratedLevels.JsonFiles.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = (await http.GetFromJsonAsync<Level>(LevelDirectory + GeneratedLevels.JsonFiles[i]))!;
            levels[i].Description = await http.GetStringAsync(levels[i].slug + ".txt");
        }
        return new(levels);
    }
}