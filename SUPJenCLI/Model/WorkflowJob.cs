using System;
using System.Text.Json.Serialization;

namespace SUPJenCLI.Model
{
    public class WorkflowJob
    {
        [JsonPropertyName("_class")]
        public string Class { get; set; }

        public Action[] Actions { get; set; }

        public object Description { get; set; }

        public string DisplayName { get; set; }

        public object DisplayNameOrNull { get; set; }

        public string FullDisplayName { get; set; }

        public string FullName { get; set; }

        public string Name { get; set; }

        public Uri Url { get; set; }

        public bool? Buildable { get; set; }

        public Build[] Builds { get; set; }

        public string Color { get; set; }

        public Build FirstBuild { get; set; }

        public HealthReport[] HealthReport { get; set; }

        public bool? InQueue { get; set; }

        public bool? KeepDependencies { get; set; }

        public Build LastBuild { get; set; }

        public Build LastCompletedBuild { get; set; }

        public object LastFailedBuild { get; set; }

        public Build LastStableBuild { get; set; }

        public Build LastSuccessfulBuild { get; set; }

        public object LastUnstableBuild { get; set; }

        public object LastUnsuccessfulBuild { get; set; }

        public long? NextBuildNumber { get; set; }

        public Property[] Property { get; set; }

        public object QueueItem { get; set; }

        public bool? ConcurrentBuild { get; set; }

        public bool? ResumeBlocked { get; set; }
    }
}
