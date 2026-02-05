<div align="center">

# ⚡ AmazePerf

### Real-Time System Performance Monitor & Freeze Detector

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.0-512BD4?style=for-the-badge&logo=.net)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=for-the-badge&logo=windows)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)
[![Version](https://img.shields.io/badge/Version-1.0.0-blue?style=for-the-badge)](https://github.com/Adiru3/AmazePerf/releases)

**Detect freezes, lags, and performance bottlenecks in real-time with intelligent AI-powered analysis and actionable solutions.**

[Download](https://github.com/Adiru3/AmazePerf/releases) • [Documentation](#-features) • [Report Bug](https://github.com/Adiru3/AmazePerf/issues) • [Support Development](https://adiru3.github.io/Donate/)

![AmazePerf Screenshot](https://via.placeholder.com/800x450/0f0f19/00c8ff?text=AmazePerf+Dashboard)

</div>

---

## 🌟 Features

### 📊 **Real-Time Performance Monitoring**
- **CPU Usage Tracking** - Monitor processor load with 60-second historical graphs
- **Memory Analysis** - Track RAM consumption and detect memory leaks
- **Disk I/O Monitoring** - Analyze read/write speeds and queue lengths
- **Live Performance Charts** - Beautiful gradient-filled charts with color-coded thresholds

### 🔍 **Intelligent Issue Detection**
- **Freeze Detection** - Automatically identify CPU spikes causing system freezes (>95%)
- **Lag Pattern Recognition** - Detect sustained high resource usage patterns
- **Bottleneck Identification** - Pinpoint disk I/O, memory, and CPU bottlenecks
- **Root Cause Analysis** - AI-powered analysis with severity classification

### 💡 **Actionable Solutions**
- **Step-by-Step Fixes** - Detailed instructions for every detected issue
- **Quick Fix Recommendations** - Immediate relief for critical problems
- **Process-Specific Advice** - Tailored solutions for browsers, antivirus, system processes
- **Optimization Tips** - Proactive recommendations to prevent future issues

### 🖥️ **Modern User Interface**
- **Dark Theme Design** - Easy on the eyes with neon accent colors
- **Real-Time Charts** - Smooth 60-second performance graphs
- **Color-Coded Alerts** - Green/Yellow/Red severity indicators
- **Process Monitor** - Top 10 processes by memory usage with status indicators

---

## 🚀 Quick Start

### Installation

#### Option 1: Download Pre-built Binary
1. Download the latest release from [Releases](https://github.com/Adiru3/AmazePerf/releases)
2. Extract `AmazePerf.exe`
3. Run the application (Administrator recommended)

#### Option 2: Build from Source
```bash
# Clone the repository
git clone https://github.com/Adiru3/AmazePerf.git
cd AmazePerf

# Build with MSBuild
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe AmazePerf.csproj /p:Configuration=Release

# Or use the build script
build.bat
```

### System Requirements
- **OS:** Windows 7/8/10/11
- **.NET Framework:** 4.0 or higher
- **Privileges:** Administrator (recommended for full functionality)
- **RAM:** 50 MB minimum
- **CPU:** Minimal impact (<5% CPU usage)

---

## 📖 How to Use

1. **Launch AmazePerf** - Run `AmazePerf.exe` (right-click → Run as Administrator for best results)
2. **Monitor Performance** - View real-time CPU, RAM, and Disk metrics in the top charts
3. **Check Issues** - The "Detected Issues" panel shows all current performance problems
4. **Get Solutions** - Click any issue to see detailed step-by-step fix instructions
5. **Monitor Processes** - Bottom panel shows top memory-consuming processes

### Understanding Severity Levels
- 🔴 **CRITICAL** - Immediate attention required (>95% resource usage)
- 🟠 **HIGH** - Significant performance impact (>80% resource usage)
- 🟡 **MEDIUM** - Noticeable but manageable issues
- 🟢 **LOW** - Minor issues with minimal impact

---

## 🛠️ Technical Details

### Architecture
```
AmazePerf/
├── PerformanceMonitor.cs    # Core monitoring engine with background thread
├── DiagnosticAnalyzer.cs    # Pattern recognition & root cause analysis
├── SolutionProvider.cs      # Knowledge base for fix recommendations
├── PerformanceChart.cs      # Custom WinForms chart control
├── MainForm.cs              # GUI with real-time updates
└── PerformanceIssue.cs      # Data model for detected issues
```

### Performance Counters Used
```
Processor\% Processor Time\_Total
Memory\% Committed Bytes In Use
PhysicalDisk\Disk Read Bytes/sec\_Total
PhysicalDisk\Disk Write Bytes/sec\_Total
PhysicalDisk\Current Disk Queue Length\_Total
```

### Detection Algorithms
- **CPU Spike Detection:** Flags sudden spikes >95% with lower average
- **Sustained Load Analysis:** Averages last 10 seconds, alerts if >80%
- **Memory Leak Detection:** Identifies rapid memory increase (>10% in 10s)
- **Disk Bottleneck:** Monitors queue length and I/O throughput
- **Process Analysis:** Flags processes using >1GB RAM

---

## 🎯 Use Cases

- ✅ **Gaming Performance** - Identify what's causing FPS drops and stuttering
- ✅ **Video Editing** - Monitor resource usage during rendering
- ✅ **Software Development** - Debug memory leaks and CPU bottlenecks
- ✅ **System Diagnostics** - Troubleshoot slow boot times and freezes
- ✅ **Server Monitoring** - Track performance on Windows servers
- ✅ **General Optimization** - Keep your PC running smoothly

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. 🐛 **Report Bugs** - [Open an issue](https://github.com/Adiru3/AmazePerf/issues)
2. 💡 **Suggest Features** - Share your ideas in discussions
3. 🔧 **Submit Pull Requests** - Fix bugs or add new features
4. 📖 **Improve Documentation** - Help others understand AmazePerf better

### Development Setup
```bash
git clone https://github.com/Adiru3/AmazePerf.git
cd AmazePerf
# Open AmazePerf.csproj in Visual Studio
# Build and run (F5)
```

---

## 💖 Support Development

If AmazePerf helps you, consider supporting its development:

<div align="center">

[![Donate](https://img.shields.io/badge/Donate-Support%20Development-yellow?style=for-the-badge&logo=paypal)](https://adiru3.github.io/Donate/)

**[☕ Buy me a coffee](https://adiru3.github.io/Donate/)**

</div>

Your support helps maintain and improve AmazePerf with new features and updates!

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Adiru3**

[![GitHub](https://img.shields.io/badge/GitHub-Adiru3-181717?style=for-the-badge&logo=github)](https://github.com/Adiru3)
[![Website](https://img.shields.io/badge/Website-Donate-FF5722?style=for-the-badge&logo=google-chrome)](https://adiru3.github.io/Donate/)

---

## 🔗 Related Projects

- [AmazeBridge](https://github.com/Adiru3/AmazeBridge) - P2P Messenger
- [More projects...](https://github.com/Adiru3?tab=repositories)

---

<div align="center">

### ⭐ Star this repository if you find it useful!

**Made with ❤️ by [Adiru3](https://github.com/Adiru3)**

[⬆ Back to Top](#-amazeperf)

</div>


