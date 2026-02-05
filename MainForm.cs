using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AmazePerf
{
    public partial class MainForm : Form
    {
        private PerformanceMonitor monitor;
        private DiagnosticAnalyzer analyzer;
        private SolutionProvider solutionProvider;
        
        private PerformanceChart cpuChart;
        private PerformanceChart ramChart;
        private PerformanceChart diskChart;
        
        private Label lblCpuValue;
        private Label lblRamValue;
        private Label lblDiskValue;
        private Label lblStatus;
        
        private ListBox lstIssues;
        private RichTextBox txtSolutions;
        private ListView lstProcesses;
        
        private Timer updateTimer;
        private Dictionary<string, PerformanceIssue> activeIssues;
        
        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            
            monitor = new PerformanceMonitor();
            analyzer = new DiagnosticAnalyzer();
            solutionProvider = new SolutionProvider();
            activeIssues = new Dictionary<string, PerformanceIssue>();
            
            monitor.DataUpdated += Monitor_DataUpdated;
            
            updateTimer = new Timer();
            updateTimer.Interval = 2000; // Analyze every 2 seconds
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
            
            monitor.Start();
        }
        
        private void InitializeComponent()
        {
            this.Text = "AmazePerf - System Performance Analyzer";
            this.Size = new Size(1200, 700);
            this.BackColor = Color.FromArgb(15, 15, 25);
            this.ForeColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            
            this.FormClosing += MainForm_FormClosing;
        }
        
        private void InitializeCustomComponents()
        {
            // Header
            Label lblTitle = new Label();
            lblTitle.Text = "AmazePerf";
            lblTitle.Font = new Font("Segoe UI", 20f, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 200, 255);
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);
            
            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Real-Time System Performance Monitor";
            lblSubtitle.Font = new Font("Segoe UI", 10f);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.Location = new Point(20, 50);
            lblSubtitle.AutoSize = true;
            this.Controls.Add(lblSubtitle);
            
            // Status indicator
            lblStatus = new Label();
            lblStatus.Text = "● Monitoring Active";
            lblStatus.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(0, 255, 100);
            lblStatus.Location = new Point(20, 75);
            lblStatus.AutoSize = true;
            this.Controls.Add(lblStatus);
            
            // Performance Charts
            int chartY = 110;
            int chartWidth = 360;
            int chartHeight = 120;
            int chartSpacing = 20;
            
            // CPU Chart
            cpuChart = new PerformanceChart();
            cpuChart.ChartTitle = "CPU Usage";
            cpuChart.LineColor = Color.FromArgb(255, 100, 100);
            cpuChart.Location = new Point(20, chartY);
            cpuChart.Size = new Size(chartWidth, chartHeight);
            this.Controls.Add(cpuChart);
            
            lblCpuValue = CreateMetricLabel("0.0%", new Point(20, chartY + chartHeight + 5));
            this.Controls.Add(lblCpuValue);
            
            // RAM Chart
            ramChart = new PerformanceChart();
            ramChart.ChartTitle = "Memory Usage";
            ramChart.LineColor = Color.FromArgb(100, 200, 255);
            ramChart.Location = new Point(20 + chartWidth + chartSpacing, chartY);
            ramChart.Size = new Size(chartWidth, chartHeight);
            this.Controls.Add(ramChart);
            
            lblRamValue = CreateMetricLabel("0.0%", new Point(20 + chartWidth + chartSpacing, chartY + chartHeight + 5));
            this.Controls.Add(lblRamValue);
            
            // Disk Chart
            diskChart = new PerformanceChart();
            diskChart.ChartTitle = "Disk Activity";
            diskChart.LineColor = Color.FromArgb(255, 200, 0);
            diskChart.MaxValue = 200; // MB/s
            diskChart.Location = new Point(20 + (chartWidth + chartSpacing) * 2, chartY);
            diskChart.Size = new Size(chartWidth, chartHeight);
            this.Controls.Add(diskChart);
            
            lblDiskValue = CreateMetricLabel("0.0 MB/s", new Point(20 + (chartWidth + chartSpacing) * 2, chartY + chartHeight + 5));
            this.Controls.Add(lblDiskValue);
            
            // Issues Panel
            int issuesPanelY = chartY + chartHeight + 40;
            
            Label lblIssuesTitle = new Label();
            lblIssuesTitle.Text = "Detected Issues";
            lblIssuesTitle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblIssuesTitle.ForeColor = Color.FromArgb(255, 100, 100);
            lblIssuesTitle.Location = new Point(20, issuesPanelY);
            lblIssuesTitle.AutoSize = true;
            this.Controls.Add(lblIssuesTitle);
            
            lstIssues = new ListBox();
            lstIssues.BackColor = Color.FromArgb(25, 25, 35);
            lstIssues.ForeColor = Color.White;
            lstIssues.Font = new Font("Consolas", 9f);
            lstIssues.Location = new Point(20, issuesPanelY + 30);
            lstIssues.Size = new Size(560, 150);
            lstIssues.BorderStyle = BorderStyle.FixedSingle;
            lstIssues.SelectedIndexChanged += LstIssues_SelectedIndexChanged;
            this.Controls.Add(lstIssues);
            
            // Solutions Panel
            Label lblSolutionsTitle = new Label();
            lblSolutionsTitle.Text = "Solutions & Fixes";
            lblSolutionsTitle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblSolutionsTitle.ForeColor = Color.FromArgb(0, 255, 100);
            lblSolutionsTitle.Location = new Point(600, issuesPanelY);
            lblSolutionsTitle.AutoSize = true;
            this.Controls.Add(lblSolutionsTitle);
            
            txtSolutions = new RichTextBox();
            txtSolutions.BackColor = Color.FromArgb(25, 25, 35);
            txtSolutions.ForeColor = Color.White;
            txtSolutions.Font = new Font("Segoe UI", 9f);
            txtSolutions.Location = new Point(600, issuesPanelY + 30);
            txtSolutions.Size = new Size(560, 150);
            txtSolutions.BorderStyle = BorderStyle.FixedSingle;
            txtSolutions.ReadOnly = true;
            this.Controls.Add(txtSolutions);
            
            // Process List
            int processListY = issuesPanelY + 190;
            
            Label lblProcessTitle = new Label();
            lblProcessTitle.Text = "Top Processes by Memory";
            lblProcessTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblProcessTitle.ForeColor = Color.FromArgb(200, 200, 255);
            lblProcessTitle.Location = new Point(20, processListY);
            lblProcessTitle.AutoSize = true;
            this.Controls.Add(lblProcessTitle);
            
            lstProcesses = new ListView();
            lstProcesses.BackColor = Color.FromArgb(25, 25, 35);
            lstProcesses.ForeColor = Color.White;
            lstProcesses.Font = new Font("Consolas", 9f);
            lstProcesses.Location = new Point(20, processListY + 30);
            lstProcesses.Size = new Size(1140, 120);
            lstProcesses.View = View.Details;
            lstProcesses.FullRowSelect = true;
            lstProcesses.GridLines = true;
            lstProcesses.BorderStyle = BorderStyle.FixedSingle;
            
            lstProcesses.Columns.Add("Process Name", 300);
            lstProcesses.Columns.Add("PID", 80);
            lstProcesses.Columns.Add("Memory (MB)", 120);
            lstProcesses.Columns.Add("Threads", 100);
            lstProcesses.Columns.Add("Status", 200);
            
            this.Controls.Add(lstProcesses);
            
            // Footer with links
            int footerY = processListY + 160;
            
            LinkLabel lnkGithub = new LinkLabel();
            lnkGithub.Text = "GitHub: Adiru3";
            lnkGithub.Font = new Font("Segoe UI", 9f);
            lnkGithub.LinkColor = Color.FromArgb(0, 200, 255);
            lnkGithub.Location = new Point(20, footerY);
            lnkGithub.AutoSize = true;
            lnkGithub.LinkClicked += (s, e) => Process.Start("https://github.com/Adiru3");
            this.Controls.Add(lnkGithub);
            
            LinkLabel lnkDonate = new LinkLabel();
            lnkDonate.Text = "Support Development (Donate)";
            lnkDonate.Font = new Font("Segoe UI", 9f);
            lnkDonate.LinkColor = Color.FromArgb(255, 200, 0);
            lnkDonate.Location = new Point(150, footerY);
            lnkDonate.AutoSize = true;
            lnkDonate.LinkClicked += (s, e) => Process.Start("https://adiru3.github.io/Donate/");
            this.Controls.Add(lnkDonate);
            
            Label lblVersion = new Label();
            lblVersion.Text = "v1.0.0 | .NET Framework 4.0";
            lblVersion.Font = new Font("Segoe UI", 8f);
            lblVersion.ForeColor = Color.Gray;
            lblVersion.Location = new Point(1000, footerY);
            lblVersion.AutoSize = true;
            this.Controls.Add(lblVersion);
        }
        
        private Label CreateMetricLabel(string text, Point location)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.Location = location;
            lbl.AutoSize = true;
            return lbl;
        }
        
        private void Monitor_DataUpdated(object sender, PerformanceDataEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<PerformanceDataEventArgs>(Monitor_DataUpdated), sender, e);
                return;
            }
            
            // Update charts
            cpuChart.AddDataPoint(e.CpuUsage);
            ramChart.AddDataPoint(e.RamUsage);
            diskChart.AddDataPoint(e.DiskRead + e.DiskWrite);
            
            // Update labels
            lblCpuValue.Text = string.Format("CPU: {0:F1}%", e.CpuUsage);
            lblCpuValue.ForeColor = GetColorForValue(e.CpuUsage, 70, 90);
            
            lblRamValue.Text = string.Format("RAM: {0:F1}%", e.RamUsage);
            lblRamValue.ForeColor = GetColorForValue(e.RamUsage, 70, 85);
            
            lblDiskValue.Text = string.Format("Disk: {0:F1} MB/s", e.DiskRead + e.DiskWrite);
            lblDiskValue.ForeColor = GetColorForValue(e.DiskRead + e.DiskWrite, 50, 100);
        }
        
        private Color GetColorForValue(float value, float warningThreshold, float criticalThreshold)
        {
            if (value >= criticalThreshold)
                return Color.FromArgb(255, 100, 100);
            else if (value >= warningThreshold)
                return Color.FromArgb(255, 200, 0);
            else
                return Color.FromArgb(0, 255, 100);
        }
        
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // Analyze system
            var issues = analyzer.AnalyzeSystem(monitor);
            
            // Update active issues
            foreach (var issue in issues)
            {
                var key = issue.Title + "_" + issue.AffectedComponent;
                if (!activeIssues.ContainsKey(key))
                {
                    solutionProvider.ProvideSolutions(issue);
                    activeIssues[key] = issue;
                    
                    // Add to list
                    string displayText = string.Format("[{0}] {1}", 
                        issue.Severity.ToString().ToUpper(), issue.Title);
                    lstIssues.Items.Add(displayText);
                    lstIssues.Tag = activeIssues; // Store reference
                }
            }
            
            // Update process list
            UpdateProcessList();
            
            // Update status
            if (activeIssues.Count > 0)
            {
                lblStatus.Text = string.Format("● {0} Issue(s) Detected", activeIssues.Count);
                lblStatus.ForeColor = Color.FromArgb(255, 200, 0);
            }
            else
            {
                lblStatus.Text = "● System Running Smoothly";
                lblStatus.ForeColor = Color.FromArgb(0, 255, 100);
            }
        }
        
        private void UpdateProcessList()
        {
            lstProcesses.Items.Clear();
            
            var processes = monitor.GetTopProcesses(10);
            foreach (var proc in processes)
            {
                var item = new ListViewItem(proc.Name);
                item.SubItems.Add(proc.Id.ToString());
                item.SubItems.Add(proc.MemoryMB.ToString("N0"));
                item.SubItems.Add(proc.ThreadCount.ToString());
                
                string status = "Normal";
                if (proc.MemoryMB > 2048)
                {
                    status = "High Memory Usage";
                    item.ForeColor = Color.FromArgb(255, 100, 100);
                }
                else if (proc.MemoryMB > 1024)
                {
                    status = "Elevated Memory";
                    item.ForeColor = Color.FromArgb(255, 200, 0);
                }
                
                item.SubItems.Add(status);
                lstProcesses.Items.Add(item);
            }
        }
        
        private void LstIssues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstIssues.SelectedIndex < 0) return;
            
            var issuesList = activeIssues.Values.ToArray();
            if (lstIssues.SelectedIndex >= issuesList.Length) return;
            
            var selectedIssue = issuesList[lstIssues.SelectedIndex];
            
            // Display solutions
            txtSolutions.Clear();
            txtSolutions.SelectionFont = new Font("Segoe UI", 11f, FontStyle.Bold);
            txtSolutions.SelectionColor = Color.FromArgb(255, 200, 0);
            txtSolutions.AppendText(selectedIssue.Title + "\n\n");
            
            txtSolutions.SelectionFont = new Font("Segoe UI", 9f);
            txtSolutions.SelectionColor = Color.White;
            txtSolutions.AppendText(selectedIssue.Description + "\n\n");
            
            txtSolutions.SelectionFont = new Font("Segoe UI", 10f, FontStyle.Bold);
            txtSolutions.SelectionColor = Color.FromArgb(0, 255, 100);
            txtSolutions.AppendText("How to Fix:\n");
            
            txtSolutions.SelectionFont = new Font("Segoe UI", 9f);
            txtSolutions.SelectionColor = Color.White;
            foreach (var solution in selectedIssue.Solutions)
            {
                txtSolutions.AppendText("\n" + solution);
            }
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            updateTimer.Stop();
            monitor.Stop();
            monitor.Dispose();
        }
    }
}
