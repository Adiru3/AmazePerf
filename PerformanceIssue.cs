using System;
using System.Collections.Generic;

namespace AmazePerf
{
    /// <summary>
    /// Represents a detected performance issue with severity and solutions
    /// </summary>
    public class PerformanceIssue
    {
        public enum IssueSeverity
        {
            Low,
            Medium,
            High,
            Critical
        }

        public enum IssueCategory
        {
            CPU,
            Memory,
            Disk,
            GPU,
            Network,
            Process,
            System
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public IssueCategory Category { get; set; }
        public IssueSeverity Severity { get; set; }
        public DateTime DetectedAt { get; set; }
        public string AffectedComponent { get; set; }
        public List<string> Solutions { get; set; }
        public Dictionary<string, object> Metrics { get; set; }

        public PerformanceIssue()
        {
            Solutions = new List<string>();
            Metrics = new Dictionary<string, object>();
            DetectedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1} - {2}", Severity, Title, Description);
        }
    }
}
