using System;
using System.Collections.Generic;

namespace AmazePerf
{
    /// <summary>
    /// Provides actionable solutions for detected performance issues
    /// </summary>
    public class SolutionProvider
    {
        public void ProvideSolutions(PerformanceIssue issue)
        {
            issue.Solutions.Clear();
            
            switch (issue.Category)
            {
                case PerformanceIssue.IssueCategory.CPU:
                    ProvideCPUSolutions(issue);
                    break;
                    
                case PerformanceIssue.IssueCategory.Memory:
                    ProvideMemorySolutions(issue);
                    break;
                    
                case PerformanceIssue.IssueCategory.Disk:
                    ProvideDiskSolutions(issue);
                    break;
                    
                case PerformanceIssue.IssueCategory.Process:
                    ProvideProcessSolutions(issue);
                    break;
                    
                default:
                    ProvideGeneralSolutions(issue);
                    break;
            }
        }
        
        private void ProvideCPUSolutions(PerformanceIssue issue)
        {
            if (issue.Title.Contains("Spike") || issue.Title.Contains("Freeze"))
            {
                issue.Solutions.Add("1. Open Task Manager (Ctrl+Shift+Esc) and check the Processes tab");
                issue.Solutions.Add("2. Sort by CPU usage to identify the problematic process");
                issue.Solutions.Add("3. End the high-CPU process if it's not critical");
                issue.Solutions.Add("4. Check for Windows Updates that might be running in background");
                issue.Solutions.Add("5. Disable startup programs: Settings > Apps > Startup");
            }
            else if (issue.Title.Contains("Sustained"))
            {
                issue.Solutions.Add("1. Identify CPU-intensive processes in Task Manager");
                issue.Solutions.Add("2. Close unnecessary applications and browser tabs");
                issue.Solutions.Add("3. Check for malware using Windows Defender");
                issue.Solutions.Add("4. Update device drivers, especially graphics and chipset");
                issue.Solutions.Add("5. Consider upgrading CPU or reducing workload");
                issue.Solutions.Add("6. Clean dust from CPU cooler to prevent thermal throttling");
            }
            else
            {
                issue.Solutions.Add("1. Close unnecessary programs to reduce CPU load");
                issue.Solutions.Add("2. Restart your computer to clear temporary processes");
                issue.Solutions.Add("3. Check CPU temperature - overheating causes throttling");
            }
        }
        
        private void ProvideMemorySolutions(PerformanceIssue issue)
        {
            if (issue.Title.Contains("Leak"))
            {
                issue.Solutions.Add("1. Identify the process with increasing memory in Task Manager");
                issue.Solutions.Add("2. Restart the problematic application");
                issue.Solutions.Add("3. Update the application to the latest version");
                issue.Solutions.Add("4. Report the issue to the software developer");
                issue.Solutions.Add("5. As a temporary fix, schedule periodic restarts of the app");
            }
            else
            {
                issue.Solutions.Add("1. Close unused applications and browser tabs");
                issue.Solutions.Add("2. Restart your computer to free up memory");
                issue.Solutions.Add("3. Increase virtual memory (pagefile) size:");
                issue.Solutions.Add("   - System Properties > Advanced > Performance Settings");
                issue.Solutions.Add("   - Advanced tab > Virtual Memory > Change");
                issue.Solutions.Add("4. Disable unnecessary startup programs");
                issue.Solutions.Add("5. Consider upgrading RAM if this happens frequently");
                issue.Solutions.Add("6. Use ReadyBoost with a USB drive (for older systems)");
            }
        }
        
        private void ProvideDiskSolutions(PerformanceIssue issue)
        {
            if (issue.Title.Contains("Bottleneck"))
            {
                issue.Solutions.Add("1. Check which process is using disk in Task Manager > Performance > Disk");
                issue.Solutions.Add("2. Disable Windows Search indexing temporarily:");
                issue.Solutions.Add("   - Services.msc > Windows Search > Stop");
                issue.Solutions.Add("3. Disable Superfetch/SysMain service (Windows 10/11)");
                issue.Solutions.Add("4. Run disk cleanup: cleanmgr.exe");
                issue.Solutions.Add("5. Defragment HDD or optimize SSD:");
                issue.Solutions.Add("   - Search 'Defragment' > Optimize Drives");
                issue.Solutions.Add("6. Check disk health with: chkdsk /f /r");
                issue.Solutions.Add("7. Upgrade to SSD for dramatic performance improvement");
            }
            else
            {
                issue.Solutions.Add("1. Identify disk-intensive processes in Task Manager");
                issue.Solutions.Add("2. Pause file transfers or downloads temporarily");
                issue.Solutions.Add("3. Disable Windows Update if it's running");
                issue.Solutions.Add("4. Free up disk space (keep at least 15% free)");
                issue.Solutions.Add("5. Consider upgrading to an SSD");
            }
        }
        
        private void ProvideProcessSolutions(PerformanceIssue issue)
        {
            var processName = issue.AffectedComponent;
            
            issue.Solutions.Add(string.Format("1. End '{0}' process in Task Manager if not needed", processName));
            issue.Solutions.Add("2. Restart the application to clear memory leaks");
            issue.Solutions.Add("3. Check if the application has updates available");
            issue.Solutions.Add("4. Reduce the workload within the application");
            
            // Specific advice for common processes
            if (processName.ToLower().Contains("chrome") || processName.ToLower().Contains("firefox") || 
                processName.ToLower().Contains("edge") || processName.ToLower().Contains("browser"))
            {
                issue.Solutions.Add("5. Close unnecessary browser tabs and extensions");
                issue.Solutions.Add("6. Clear browser cache and cookies");
                issue.Solutions.Add("7. Disable hardware acceleration in browser settings");
            }
            else if (processName.ToLower().Contains("antimalware") || processName.ToLower().Contains("defender"))
            {
                issue.Solutions.Add("5. Schedule scans for off-peak hours");
                issue.Solutions.Add("6. Add exclusions for trusted folders");
            }
            else if (processName.ToLower().Contains("system"))
            {
                issue.Solutions.Add("5. This is a Windows process - check for Windows Updates");
                issue.Solutions.Add("6. Run System File Checker: sfc /scannow");
            }
        }
        
        private void ProvideGeneralSolutions(PerformanceIssue issue)
        {
            issue.Solutions.Add("1. Restart your computer");
            issue.Solutions.Add("2. Check for Windows Updates");
            issue.Solutions.Add("3. Run Windows Performance Troubleshooter");
            issue.Solutions.Add("4. Update all device drivers");
            issue.Solutions.Add("5. Scan for malware with Windows Defender");
        }
        
        public string GetQuickFix(PerformanceIssue issue)
        {
            switch (issue.Category)
            {
                case PerformanceIssue.IssueCategory.CPU:
                    return "Quick Fix: Close unnecessary programs and check Task Manager for CPU hogs";
                    
                case PerformanceIssue.IssueCategory.Memory:
                    return "Quick Fix: Close unused applications and restart your computer";
                    
                case PerformanceIssue.IssueCategory.Disk:
                    return "Quick Fix: Pause file transfers and disable Windows Search temporarily";
                    
                case PerformanceIssue.IssueCategory.Process:
                    return string.Format("Quick Fix: End '{0}' process in Task Manager", issue.AffectedComponent);
                    
                default:
                    return "Quick Fix: Restart your computer to clear temporary issues";
            }
        }
    }
}
