using System;
using System.Text.Json.Serialization;

namespace SUPJenCLI.Model
{
    public class Action
    {
        [JsonPropertyName("_class")]
        public string Class { get; set; }

        public long? BlockedDurationMillis { get; set; }

        public long? BlockedTimeMillis { get; set; }

        public long? BuildableDurationMillis { get; set; }

        public long? BuildableTimeMillis { get; set; }

        public long? BuildingDurationMillis { get; set; }

        public long? ExecutingTimeMillis { get; set; }

        public double? ExecutorUtilization { get; set; }

        public long? SubTaskCount { get; set; }

        public long? WaitingDurationMillis { get; set; }

        public long? WaitingTimeMillis { get; set; }

        public Uri[] RemoteUrls { get; set; }

        public string ScmName { get; set; }

        public long? FailCount { get; set; }

        public long? SkipCount { get; set; }

        public long? TotalCount { get; set; }

        public string UrlName { get; set; }
    }
}
