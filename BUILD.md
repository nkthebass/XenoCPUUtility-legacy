# Build Instructions - XenoCPUUtility Legacy

## Quick Start

The legacy version is located in `XenoCPUUtility-Legacy/` and is ready to compile.

### Option 1: Using Visual Studio (Easiest)

1. Open `XenoCPUUtility-Legacy/XenoCPUUtility-Legacy.sln` in Visual Studio 2010 or later
2. Select **Build → Build Solution** (or press Ctrl+Shift+B)
3. The compiled .exe will be in `bin/Release/XenoCPUUtilityLegacy.exe`

### Option 2: Using MSBuild Command Line

```bash
# Navigate to the legacy project directory
cd XenoCPUUtility-Legacy

# Build Release version
msbuild XenoCPUUtility-Legacy.csproj /p:Configuration=Release /p:Platform=AnyCPU

# The output will be in bin/Release/XenoCPUUtilityLegacy.exe
```

### Option 3: Using .NET Framework Build Tools

```bash
# Make sure you have .NET Framework 4.0 installed
"%ProgramFiles(x86)%\MSBuild\12.0\Bin\msbuild.exe" XenoCPUUtility-Legacy.csproj /p:Configuration=Release
```

## System Requirements for Building

- **Visual Studio 2010** or higher (Community, Professional, or Enterprise)
- **OR**: .NET Framework 4.0 SDK + MSBuild tools
- **.NET Framework 4.0 Target Pack** (for IntelliSense and project support)

## Running the Program

### On Windows XP SP3 or Vista
1. Ensure **.NET Framework 4.0** is installed (can download from Microsoft)
2. Run `XenoCPUUtilityLegacy.exe`

### On Windows 7 and Later
1. .NET Framework 4.0+ is built-in or easily available via Windows Update
2. Run `XenoCPUUtilityLegacy.exe`

## Project Structure

```
XenoCPUUtility-Legacy/
├── XenoCPUUtility-Legacy.sln           # Solution file
├── XenoCPUUtility-Legacy.csproj        # Project file
├── Program.cs                          # Entry point
├── MainForm.cs                         # WinForms UI
├── MainForm.Designer.cs                # UI Designer (auto-generated)
├── BenchmarkEngine.cs                  # Single/Multi-core benchmark
├── StressEngine.cs                     # Heavy Load & Instability Check
├── PathTracerBenchmark.cs              # Ray tracing benchmark
├── HardwareMetrics.cs                  # CPU info & metrics
├── Properties/
│   └── AssemblyInfo.cs
├── README.md                           # Feature documentation
├── bin/
│   ├── Debug/                          # Debug build output
│   └── Release/                        # Release build output (use this)
└── obj/
    ├── Debug/                          # Intermediate files
    └── Release/
```

## What's Included

### Core Benchmarking
- **Single-Core Benchmark**: FP-heavy workload (~10 seconds)
- **Multi-Core Benchmark**: Parallel scaling benchmark (~6 seconds)
- **Path Tracing**: CPU-intensive ray tracing (configurable resolution)

### Stress Testing
- **Heavy Load Mode**: 100% CPU saturation with sqrt/sin/cos
- **Instability Check Mode**: 4-phase rotating workload (FP → Integer → Memory → Mixed)
  - Includes idle dips to detect voltage regulation issues

### Features
- Start/Stop/Pause/Resume controls
- Real-time status display
- Timestamped results log
- CPU model and core count detection
- Thread-safe operation on legacy OS

## Troubleshooting

### "System.Management not found"
- Ensure you're targeting .NET Framework 4.0 or higher
- Add `using System.Management;` reference

### Build fails with "The project file is invalid"
- Ensure you have .NET Framework 4.0 Target Pack installed in Visual Studio
- Try using MSBuild directly from command line

### Application crashes on startup (Win XP)
- Verify .NET Framework 4.0 is installed
- Download from: https://www.microsoft.com/en-us/download/details.aspx?id=17718

### Performance lower than expected
- Ensure no other heavy processes are running
- Run benchmarks in sequence, not simultaneously
- Close background applications

## Differences from v1.9.4

| Feature | Modern | Legacy |
|---------|--------|--------|
| UI Framework | WebView2 | WinForms |
| .NET Target | .NET 8 | .NET Framework 4.0 |
| OS Support | Win 10/11+ | XP/Vista/7/8+ |
| Single-Core Bench | ✓ | ✓ |
| Multi-Core Bench | ✓ | ✓ |
| Path Tracer | ✓ | ✓ (Simplified) |
| Heavy Load Stress | ✓ | ✓ |
| Instability Check | ✓ | ✓ |
| RAM Stress | ✓ | ✗ |
| WHEA Monitoring | ✓ | ✗ |
| Charts/Graphs | ✓ | ✗ |
| Web UI | ✓ | ✗ |

## Development Notes

- The code uses .NET 4.0 compatible patterns (no LINQ expressions, no async/await for UI)
- `ManualResetEvent` is used instead of `ManualResetEventSlim` for compatibility
- Vector3 math is self-contained in PathTracerBenchmark.cs (no external deps)
- All stress test threads use simple loops to avoid blocking issues on older CLR

## Performance Expectations

**Single-Core Score**: 50-100 (relative to reference hardware)
**Multi-Core Score**: 200-1000+ (depends on core count and frequency)
**Path Tracer**: 100-1000 pixels/second (320x240 resolution, 32 SPP)

## License

Ported from XenoCPUUtility by nkthebass
Created for compatibility with legacy Windows versions
