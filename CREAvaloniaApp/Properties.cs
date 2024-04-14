using Newtonsoft.Json;

namespace CREAvaloniaApp
{
    public class Properties
    {
        public Resolution_ Resolution = new();
        public class Resolution_
        {
            public int x = 1980;
            public int y = 1080;
        }

        [JsonIgnore]
        public static Properties Instance { get; set; } = new();
    }
}
