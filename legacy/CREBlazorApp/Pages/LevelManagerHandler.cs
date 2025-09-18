using CREBlazorApp.GeneratedCode;
using CRECSharpInterpreter.Levels;
using Newtonsoft.Json;

namespace CREBlazorApp.Pages;

public static class LevelManagerHandler
{
    public static LevelManager NewLevelManager()
    {
        Level[] levels = new Level[GeneratedLevels.JsonLevels.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = JsonConvert.DeserializeObject<Level>(GeneratedLevels.JsonLevels[i])!;
            levels[i].Description = GeneratedLevels.Descriptions[i];
            levels[i].Solution = GeneratedLevels.Solutions[i];
        }
        return new(levels);
    }
}