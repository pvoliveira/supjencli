using System;
using System.Text.Json.Serialization;

namespace SUPJenCLI.Model
{
    public class Folder
    {
        [JsonPropertyName("_class")]
        public string Class { get; set; }

        public Action[] Actions { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public object DisplayNameOrNull { get; set; }

        public string FullDisplayName { get; set; }

        public string FullName { get; set; }

        public string Name { get; set; }

        public Uri Url { get; set; }

        public View[] Jobs { get; set; }

        public View PrimaryView { get; set; }

        public View[] Views { get; set; }
    }
}
