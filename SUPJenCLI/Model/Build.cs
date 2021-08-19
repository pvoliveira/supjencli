using System;
using System.Text.Json.Serialization;

namespace SUPJenCLI.Model
{
    public class Build
    {
        [JsonPropertyName("_class")]
        public string Class { get; set; }

        public long? Number { get; set; }

        public Uri Url { get; set; }
    }
}
