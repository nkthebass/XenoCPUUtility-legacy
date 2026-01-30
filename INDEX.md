# XenoCPUUtility Legacy - Getting Started

## 📋 What You Need to Know

I've successfully created a **Windows XP/Vista/7 compatible version** of XenoCPUUtility with all the core functionality:

### ✅ What's Included
- **3 Benchmarks**: Single-Core, Multi-Core, Path Tracing
- **2 Stress Tests**: Heavy Load + Instability Check (4-phase rotating workload)
- **Simple WinForms UI** (no WebView2 required)
- **Pause/Resume controls**
- **Real-time metrics**

### 🎯 Key Files to Read

1. **[SUMMARY.md](SUMMARY.md)** ← Start here! Complete overview
2. **[README.md](README.md)** - Features and architecture
3. **[BUILD.md](BUILD.md)** - Detailed build instructions
4. **[XenoCPUUtility-Legacy.sln](XenoCPUUtility-Legacy.sln)** - Open in Visual Studio

### 🔧 Quick Build

```bash
# Option 1: Visual Studio
Open XenoCPUUtility-Legacy.sln → Press Ctrl+Shift+B

# Option 2: Command Line
cd XenoCPUUtility-Legacy
msbuild XenoCPUUtility-Legacy.csproj /p:Configuration=Release
```

Output: `bin/Release/XenoCPUUtilityLegacy.exe`

### 💻 System Requirements

**To Build:**
- Visual Studio 2010+ OR MSBuild + .NET Framework 4.0 SDK
- Windows (any version)

**To Run:**
- Windows XP SP3 or later
- .NET Framework 4.0 (available for all Windows versions)

### 📂 Project Layout

```
XenoCPUUtility-Legacy/
├── Program.cs                 # Entry point
├── MainForm.cs                # UI with buttons and results
├── BenchmarkEngine.cs         # Single/Multi-core benchmarks
├── StressEngine.cs            # Stress tests (Heavy Load + Instability)
├── PathTracerBenchmark.cs     # Ray tracing workload
├── HardwareMetrics.cs         # CPU detection
├── XenoCPUUtility-Legacy.csproj
├── XenoCPUUtility-Legacy.sln
├── SUMMARY.md                 # Complete project overview
├── README.md                  # Features
├── BUILD.md                   # Build guide
└── This file
```

### 🚀 What Each Component Does

| File | Purpose |
|------|---------|
| **BenchmarkEngine.cs** | Runs single-core and multi-core benchmarks using FP math operations |
| **StressEngine.cs** | Heavy Load (continuous sqrt/sin/cos) and Instability Check (4-phase rotating) modes |
| **PathTracerBenchmark.cs** | CPU-intensive ray tracing with custom Vector3 math |
| **HardwareMetrics.cs** | Detects CPU model, core count, and frequency via WMI |
| **MainForm.cs** | Simple WinForms UI with buttons and results display |

### 📊 Benchmarks Included

1. **Single-Core**: ~10 second floating-point intensive test
2. **Multi-Core**: Parallel workload that scales with core count
3. **Path Tracer**: 320x240 ray tracing at 32 samples/pixel

### 🔴 Stress Tests Included

**Heavy Load Mode**
- Continuous sqrt/sin/cos loop
- 95-100% CPU saturation
- No thread synchronization overhead

**Instability Check Mode**  
- 5-phase rotating workload (each 2 seconds):
  - Phase 0: FP-heavy (sqrt/sin/cos)
  - Phase 1: Integer operations
  - Phase 2: Memory operations
  - Phase 3: Mixed workload
  - Phase 4: Idle (detects voltage droop)

### ⚡ Performance Expectations

- **Single-Core Score**: 50-100 (relative)
- **Multi-Core Score**: 200-1000+ (depends on cores)
- **Path Tracer**: 100-1000 pixels/sec
- **Stress Test**: Scales with CPU core count

### ❓ FAQ

**Q: Will this run on Windows XP?**
A: Yes, if .NET Framework 4.0 is installed.

**Q: Can I add RAM stress module?**
A: Yes, it's optional. The code from original Form1.cs can be ported if needed.

**Q: Why no WebView2?**
A: WebView2 requires Windows 10+. WinForms works on all versions.

**Q: Can I modify it?**
A: Absolutely! All code is straightforward C# - feel free to customize.

### 📞 Technical Notes

- **Language**: C# .NET Framework 4.0
- **UI Framework**: WinForms (no external dependencies)
- **Threading**: Task-based with ManualResetEvent for pause/resume
- **Math**: Self-contained Vector3 struct in PathTracerBenchmark

### 🔗 Original vs. Legacy

| Feature | Original (v1.9.4) | Legacy |
|---------|------------------|--------|
| OS Support | Win 10/11 | XP/Vista/7+ |
| .NET | .NET 8 | .NET 4.0 |
| UI | WebView2 + HTML | WinForms |
| Single-Core | ✓ | ✓ |
| Multi-Core | ✓ | ✓ |
| Path Tracer | ✓ | ✓ |
| Heavy Load | ✓ | ✓ |
| Instability | ✓ | ✓ |
| RAM Stress | ✓ | (optional) |

---

**Ready to build?** → Open **BUILD.md** for step-by-step instructions!

**Want all details?** → Read **SUMMARY.md** for complete technical overview!
