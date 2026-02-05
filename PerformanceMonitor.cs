using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AmazePerf
{
    /// <summary>
    /// Core performance monitoring engine that tracks system metrics in real-time
    /// </summary>
    public class PerformanceMonitor
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private PerformanceCounter diskReadCounter;
        private PerformanceCounter diskWriteCounter;
        private PerformanceCounter diskQueueCounter;
        
        private Thread monitoringThread;
        private bool isRunning;
        
        // Current metrics
        public float CurrentCpuUsage { get; private set; }
        public float CurrentRamUsage { get; private set; }
        public float CurrentDiskRead { get; private set; }
        public float CurrentDiskWrite { get; private set; }
        public float CurrentDiskQueue { get; private set; }
        
        // Historical data (last 60 seconds)
        public Queue<float> CpuHistory { get; private set; }
        public Queue<float> RamHistory { get; private set; }
        public Queue<float> DiskHistory { get; private set; }
        
        private const int HISTORY_SIZE = 60;
        
        // Events
        public event EventHandler<PerformanceDataEventArgs> DataUpdated;
        public event EventHandler<PerformanceIssue> IssueDetected;
        
        public PerformanceMonitor()
        {
            CpuHistory = new Queue<float>(HISTORY_SIZE);
            RamHistory = new Queue<float>(HISTORY_SIZE);
            DiskHistory = new Queue<float>(HISTORY_SIZE);
            
            InitializeCounters();
        }
        
        private void InitializeCounters()
        {
            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
                
                // Disk counters - using PhysicalDisk _Total
                diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
                diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
                diskQueueCounter = new PerformanceCounter("PhysicalDisk", "Current Disk Queue Length", "_Total");
                
                // Initial read to initialize counters
                cpuCounter.NextValue();
                ramCounter.NextValue();
                diskReadCounter.NextValue();
                diskWriteCounter.NextValue();
                diskQueueCounter.NextValue();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error initializing performance counters: " + ex.Message);
            }
        }
        
        public void Start()
        {
            if (isRunning) return;
            
            isRunning = true;
            monitoringThread = new Thread(MonitoringLoop);
            monitoringThread.IsBackground = true;
            monitoringThread.Start();
        }
        
        public void Stop()
        {
            isRunning = false;
            if (monitoringThread != null && monitoringThread.IsAlive)
            {
                monitoringThread.Join(2000);
            }
        }
        
        private void MonitoringLoop()
        {
            while (isRunning)
            {
                try
                {
                    // Collect current metrics
                    CurrentCpuUsage = cpuCounter.NextValue();
                    CurrentRamUsage = ramCounter.NextValue();
                    CurrentDiskRead = diskReadCounter.NextValue() / (1024 * 1024); // Convert to MB/s
                    CurrentDiskWrite = diskWriteCounter.NextValue() / (1024 * 1024); // Convert to MB/s
                    CurrentDiskQueue = diskQueueCounter.NextValue();
                    
                    // Update history
                    UpdateHistory(CpuHistory, CurrentCpuUsage);
                    UpdateHistory(RamHistory, CurrentRamUsage);
                    UpdateHistory(DiskHistory, CurrentDiskRead + CurrentDiskWrite);
                    
                    // Analyze for issues
                    AnalyzePerformance();
                    
                    // Notify listeners
                    OnDataUpdated();
                    
                    Thread.Sleep(1000); // Update every second
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error in monitoring loop: " + ex.Message);
                }
            }
        }
        
        private void UpdateHistory(Queue<float> history, float value)
        {
            if (history.Count >= HISTORY_SIZE)
            {
                history.Dequeue();
            }
            history.Enqueue(value);
        }
        
        private void AnalyzePerformance()
        {
            // Detect high CPU usage
            if (CurrentCpuUsage > 90)
            {
                var avgCpu = CpuHistory.Count > 0 ? CpuHistory.Average() : 0;
                if (avgCpu > 80) // Sustained high CPU
                {
                    var issue = new PerformanceIssue
                    {
                        Title = "High CPU Usage Detected",
                        Description = string.Format("CPU usage is at {0:F1}% (avg: {1:F1}%)", CurrentCpuUsage, avgCpu),
                        Category = PerformanceIssue.IssueCategory.CPU,
                        Severity = CurrentCpuUsage > 95 ? PerformanceIssue.IssueSeverity.Critical : PerformanceIssue.IssueSeverity.High,
                        AffectedComponent = "System CPU"
                    };
                    OnIssueDetected(issue);
                }
            }
            
            // Detect high memory usage
            if (CurrentRamUsage > 85)
            {
                var issue = new PerformanceIssue
                {
                    Title = "High Memory Usage Detected",
                    Description = string.Format("Memory usage is at {0:F1}%", CurrentRamUsage),
                    Category = PerformanceIssue.IssueCategory.Memory,
                    Severity = CurrentRamUsage > 95 ? PerformanceIssue.IssueSeverity.Critical : PerformanceIssue.IssueSeverity.High,
                    AffectedComponent = "System RAM"
                };
                OnIssueDetected(issue);
            }
            
            // Detect disk bottleneck
            if (CurrentDiskQueue > 2)
            {
                var issue = new PerformanceIssue
                {
                    Title = "Disk Bottleneck Detected",
                    Description = string.Format("Disk queue length is {0:F1} (high I/O wait)", CurrentDiskQueue),
                    Category = PerformanceIssue.IssueCategory.Disk,
                    Severity = CurrentDiskQueue > 5 ? PerformanceIssue.IssueSeverity.High : PerformanceIssue.IssueSeverity.Medium,
                    AffectedComponent = "Physical Disk"
                };
                OnIssueDetected(issue);
            }
        }
        
        public List<ProcessInfo> GetTopProcesses(int count = 10)
        {
            var processes = new List<ProcessInfo>();
            
            try
            {
                foreach (var proc in Process.GetProcesses())
                {
                    try
                    {
                        var info = new ProcessInfo
                        {
                            Name = proc.ProcessName,
                            Id = proc.Id,
                            CpuUsage = 0, // Will be calculated separately
                            MemoryMB = proc.WorkingSet64 / (1024 * 1024),
                            ThreadCount = proc.Threads.Count
                        };
                        processes.Add(info);
                    }
                    catch
                    {
                        // Skip processes we can't access
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getting processes: " + ex.Message);
            }
            
            return processes.OrderByDescending(p => p.MemoryMB).Take(count).ToList();
        }
        
        protected virtual void OnDataUpdated()
        {
            if (DataUpdated != null)
            {
                DataUpdated(this, new PerformanceDataEventArgs
                {
                    CpuUsage = CurrentCpuUsage,
                    RamUsage = CurrentRamUsage,
                    DiskRead = CurrentDiskRead,
                    DiskWrite = CurrentDiskWrite,
                    DiskQueue = CurrentDiskQueue
                });
            }
        }
        
        protected virtual void OnIssueDetected(PerformanceIssue issue)
        {
            if (IssueDetected != null)
            {
                IssueDetected(this, issue);
            }
        }
        
        public void Dispose()
        {
            Stop();
            
            if (cpuCounter != null) cpuCounter.Dispose();
            if (ramCounter != null) ramCounter.Dispose();
            if (diskReadCounter != null) diskReadCounter.Dispose();
            if (diskWriteCounter != null) diskWriteCounter.Dispose();
            if (diskQueueCounter != null) diskQueueCounter.Dispose();
        }
    }
    
    public class PerformanceDataEventArgs : EventArgs
    {
        public float CpuUsage { get; set; }
        public float RamUsage { get; set; }
        public float DiskRead { get; set; }
        public float DiskWrite { get; set; }
        public float DiskQueue { get; set; }
    }
    
    public class ProcessInfo
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public float CpuUsage { get; set; }
        public long MemoryMB { get; set; }
        public int ThreadCount { get; set; }
    }
}
