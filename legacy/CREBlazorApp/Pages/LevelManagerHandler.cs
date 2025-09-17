using CREBlazorApp.GeneratedCode;
using CRECSharpInterpreter.Levels;
using Newtonsoft.Json;

namespace CREBlazorApp.Pages;

public static class LevelManagerHandler
{
    public const string LevelDirectory = "Files/Levels/";

    public static async Task<LevelManager> NewLevelManager(HttpClient http)
    {
        Level[] levels = new Level[GeneratedLevels.JsonFiles.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            string jsonStr = await http.GetStringAsync(LevelDirectory + GeneratedLevels.JsonFiles[i]);
            levels[i] = JsonConvert.DeserializeObject<Level>(jsonStr)!;
            levels[i].Description = await http.GetStringAsync(LevelDirectory + levels[i].slug + ".txt");
        }
        return new(levels);
    }
}