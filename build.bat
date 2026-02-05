@echo off
echo ========================================
echo AmazePerf - Build Script
echo ========================================
echo.

REM Set .NET Framework 4.0 path
set DOTNET_PATH=C:\Windows\Microsoft.NET\Framework\v4.0.30319

REM Check if csc.exe exists
if not exist "%DOTNET_PATH%\csc.exe" (
    echo ERROR: .NET Framework 4.0 compiler not found!
    echo Please install .NET Framework 4.0
    pause
    exit /b 1
)

echo Compiling AmazePerf...
echo.

REM Create output directory
if not exist "bin\Release" mkdir bin\Release

REM Compile the application
"%DOTNET_PATH%\csc.exe" /target:winexe ^
    /out:bin\Release\AmazePerf.exe ^
    /win32icon:app.ico ^
    /reference:System.dll ^
    /reference:System.Core.dll ^
    /reference:System.Data.dll ^
    /reference:System.Drawing.dll ^
    /reference:System.Windows.Forms.dll ^
    /reference:System.Management.dll ^
    /reference:System.Xml.dll ^
    /optimize+ ^
    Program.cs ^
    MainForm.cs ^
    PerformanceMonitor.cs ^
    DiagnosticAnalyzer.cs ^
    SolutionProvider.cs ^
    PerformanceIssue.cs ^
    PerformanceChart.cs ^
    Properties\AssemblyInfo.cs

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo Build successful!
    echo Output: bin\Release\AmazePerf.exe
    echo ========================================
    echo.
    echo Running AmazePerf...
    start bin\Release\AmazePerf.exe
) else (
    echo.
    echo ========================================
    echo Build failed! Check errors above.
    echo ========================================
    pause
)
