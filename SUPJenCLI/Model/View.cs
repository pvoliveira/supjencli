using System;
using System.Text.Json.Serialization;

namespace SUPJenCLI.Model
{
    public class View
    {
        [JsonPropertyName("_class")]
        public string Class { get; set; }

        public string Name { get; set; }

        public Uri Url { get; set; }

        public string Color { get; set; }
    }
}
