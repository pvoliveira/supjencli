using System.Collections.Generic;
using SUPJenCLI.Model.Enum;

namespace SUPJenCLI.Model
{
    internal static class MapClasses
    {
        public static Dictionary<string, ClassType> Map => new()
        {
            ["hudson.model.Hudson"] = ClassType.Main,
            ["com.cloudbees.hudson.plugins.folder.Folder"] = ClassType.Folder,
            ["org.jenkinsci.plugins.workflow.multibranch.WorkflowMultiBranchProject"] = ClassType.Folder,
            ["org.jenkinsci.plugins.workflow.job.WorkflowJob"] = ClassType.WorkflowJob,
            ["org.jenkinsci.plugins.workflow.job.WorkflowRun"] = ClassType.WorkflowRun,
        };
    }
}
