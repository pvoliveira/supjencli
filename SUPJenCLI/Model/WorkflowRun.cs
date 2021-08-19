using System;
using System.Text.Json.Serialization;

namespace SUPJenCLI.Model
{
    public class WorkflowRun
    {
        [JsonPropertyName("_class")]
        public string Class { get; set; }

        public bool? Building { get; set; }

        public object Description { get; set; }

        public string DisplayName { get; set; }

        public long? Duration { get; set; }

        public long? EstimatedDuration { get; set; }

        public object Executor { get; set; }

        public string FullDisplayName { get; set; }

        public long? Id { get; set; }

        public bool? KeepLog { get; set; }

        public long? Number { get; set; }

        public long? QueueId { get; set; }

        public string Result { get; set; }

        public long? Timestamp { get; set; }

        public Uri Url { get; set; }

        public object[] ChangeSets { get; set; }
    }
}
