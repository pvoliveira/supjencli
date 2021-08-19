using System.Text.Json.Serialization;

namespace SUPJenCLI.Model
{
    public class Property
    {
        [JsonPropertyName("_class")]
        public string Class { get; set; }

        public Branch Branch { get; set; }
    }
}
