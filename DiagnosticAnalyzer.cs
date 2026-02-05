using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AmazePerf
{
    /// <summary>
    /// Analyzes performance data to identify root causes of issues
    /// </summary>
    public class DiagnosticAnalyzer
    {
        private Dictionary<string, DateTime> issueCache;
        private const int ISSUE_COOLDOWN_SECONDS = 30; // Don't report same issue within 30 seconds
        
        public DiagnosticAnalyzer()
        {
            issueCache = new Dictionary<string, DateTime>();
        }
        
        public List<PerformanceIssue> AnalyzeSystem(PerformanceMonitor monitor)
        {
            var issues = new List<PerformanceIssue>();
            
            // Analyze CPU patterns
            issues.AddRange(AnalyzeCPU(monitor));
            
            // Analyze memory patterns
            issues.AddRange(AnalyzeMemory(monitor));
            
            // Analyze disk patterns
            issues.AddRange(AnalyzeDisk(monitor));
            
            // Analyze processes
            issues.AddRange(AnalyzeProcesses(monitor));
            
            // Filter out recently reported issues
            return issues.Where(i => !IsRecentlyReported(i)).ToList();
        }
        
        private List<PerformanceIssue> AnalyzeCPU(PerformanceMonitor monitor)
        {
            var issues = new List<PerformanceIssue>();
            
            if (monitor.CpuHistory.Count < 10) return issues;
            
            var recent = monitor.CpuHistory.Skip(Math.Max(0, monitor.CpuHistory.Count - 10)).ToList();
            var avg = recent.Average();
            var max = recent.Max();
            
            // Sustained high CPU
            if (avg > 80)
            {
                var issue = new PerformanceIssue
                {
                    Title = "Sustained High CPU Usage",
                    Description = string.Format("CPU has been averaging {0:F1}% over the last 10 seconds", avg),
                    Category = PerformanceIssue.IssueCategory.CPU,
                    Severity = avg > 90 ? PerformanceIssue.IssueSeverity.Critical : PerformanceIssue.IssueSeverity.High,
                    AffectedComponent = "System CPU"
                };
                issue.Metrics["AverageCPU"] = avg;
                issue.Metrics["MaxCPU"] = max;
                issues.Add(issue);
            }
            
            // CPU spikes (freeze detection)
            if (max > 95 && avg < 70)
            {
                var issue = new PerformanceIssue
                {
                    Title = "CPU Spike Detected (Potential Freeze)",
                    Description = string.Format("CPU spiked to {0:F1}% causing potential system freeze", max),
                    Category = PerformanceIssue.IssueCategory.CPU,
                    Severity = PerformanceIssue.IssueSeverity.High,
                    AffectedComponent = "System CPU"
                };
                issue.Metrics["SpikeCPU"] = max;
                issues.Add(issue);
            }
            
            return issues;
        }
        
        private List<PerformanceIssue> AnalyzeMemory(PerformanceMonitor monitor)
        {
            var issues = new List<PerformanceIssue>();
            
            if (monitor.RamHistory.Count < 10) return issues;
            
            var recent = monitor.RamHistory.Skip(Math.Max(0, monitor.RamHistory.Count - 10)).ToList();
            var avg = recent.Average();
            var trend = recent.Last() - recent.First();
            
            // High memory usage
            if (avg > 85)
            {
                var issue = new PerformanceIssue
                {
                    Title = "High Memory Usage",
                    Description = string.Format("Memory usage is at {0:F1}%", avg),
                    Category = PerformanceIssue.IssueCategory.Memory,
                    Severity = avg > 95 ? PerformanceIssue.IssueSeverity.Critical : PerformanceIssue.IssueSeverity.High,
                    AffectedComponent = "System RAM"
                };
                issue.Metrics["AverageRAM"] = avg;
                issues.Add(issue);
            }
            
            // Memory leak detection (rapid increase)
            if (trend > 10 && avg > 70)
            {
                var issue = new PerformanceIssue
                {
                    Title = "Potential Memory Leak",
                    Description = string.Format("Memory usage increased by {0:F1}% in 10 seconds", trend),
                    Category = PerformanceIssue.IssueCategory.Memory,
                    Severity = PerformanceIssue.IssueSeverity.High,
                    AffectedComponent = "System RAM"
                };
                issue.Metrics["MemoryTrend"] = trend;
                issues.Add(issue);
            }
            
            return issues;
        }
        
        private List<PerformanceIssue> AnalyzeDisk(PerformanceMonitor monitor)
        {
            var issues = new List<PerformanceIssue>();
            
            // Disk queue length indicates I/O bottleneck
            if (monitor.CurrentDiskQueue > 2)
            {
                var issue = new PerformanceIssue
                {
                    Title = "Disk I/O Bottleneck",
                    Description = string.Format("Disk queue length is {0:F1}, causing system slowdown", monitor.CurrentDiskQueue),
                    Category = PerformanceIssue.IssueCategory.Disk,
                    Severity = monitor.CurrentDiskQueue > 5 ? PerformanceIssue.IssueSeverity.Critical : PerformanceIssue.IssueSeverity.High,
                    AffectedComponent = "Physical Disk"
                };
                issue.Metrics["QueueLength"] = monitor.CurrentDiskQueue;
                issue.Metrics["ReadSpeed"] = monitor.CurrentDiskRead;
                issue.Metrics["WriteSpeed"] = monitor.CurrentDiskWrite;
                issues.Add(issue);
            }
            
            // High disk activity
            var totalDiskActivity = monitor.CurrentDiskRead + monitor.CurrentDiskWrite;
            if (totalDiskActivity > 100) // More than 100 MB/s
            {
                var issue = new PerformanceIssue
                {
                    Title = "High Disk Activity",
                    Description = string.Format("Disk I/O is at {0:F1} MB/s", totalDiskActivity),
                    Category = PerformanceIssue.IssueCategory.Disk,
                    Severity = PerformanceIssue.IssueSeverity.Medium,
                    AffectedComponent = "Physical Disk"
                };
                issue.Metrics["TotalIO"] = totalDiskActivity;
                issues.Add(issue);
            }
            
            return issues;
        }
        
        private List<PerformanceIssue> AnalyzeProcesses(PerformanceMonitor monitor)
        {
            var issues = new List<PerformanceIssue>();
            
            try
            {
                var topProcesses = monitor.GetTopProcesses(5);
                
                foreach (var proc in topProcesses)
                {
                    // Flag processes using excessive memory
                    if (proc.MemoryMB > 1024) // More than 1GB
                    {
                        var issue = new PerformanceIssue
                        {
                            Title = "High Memory Process Detected",
                            Description = string.Format("{0} is using {1:F0} MB of memory", proc.Name, proc.MemoryMB),
                            Category = PerformanceIssue.IssueCategory.Process,
                            Severity = proc.MemoryMB > 2048 ? PerformanceIssue.IssueSeverity.High : PerformanceIssue.IssueSeverity.Medium,
                            AffectedComponent = proc.Name
                        };
                        issue.Metrics["ProcessMemory"] = proc.MemoryMB;
                        issue.Metrics["ProcessID"] = proc.Id;
                        issues.Add(issue);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error analyzing processes: " + ex.Message);
            }
            
            return issues;
        }
        
        private bool IsRecentlyReported(PerformanceIssue issue)
        {
            var key = issue.Title + "_" + issue.AffectedComponent;
            
            if (issueCache.ContainsKey(key))
            {
                var lastReported = issueCache[key];
                if ((DateTime.Now - lastReported).TotalSeconds < ISSUE_COOLDOWN_SECONDS)
                {
                    return true;
                }
            }
            
            issueCache[key] = DateTime.Now;
            return false;
        }
        
        public void ClearCache()
        {
            issueCache.Clear();
        }
    }
}
