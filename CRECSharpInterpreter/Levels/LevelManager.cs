using Newtonsoft.Json;
using System.IO;

namespace CRECSharpInterpreter.Levels
{
    public class LevelManager
    {
        public LevelManager()
        {
            string[] files = Directory.GetFiles(@"..\..\..\..\Files\Levels");
            Levels = new Level[files.Length];
            for (int i = 0; i < files.Length; i++)
                Levels[i] = JsonConvert.DeserializeObject<Level>(File.ReadAllText(files[i]))!;
        }

        public Level[] Levels { get; init; }
    }
}
