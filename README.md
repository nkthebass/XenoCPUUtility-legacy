# XenoCPUUtility Legacy Edition

A backport of **XenoCPUUtility** benchmarking and stress testing tools for **Windows XP, Vista, and Windows 7**.

## Features

This legacy version includes all the core performance testing functionality:

### Benchmarks
- **Single-Core Benchmark** - Measures peak single-thread floating-point performance
- **Multi-Core Benchmark** - Aggregates all CPU threads for total throughput  
- **Path Tracing Benchmark** - CPU-intensive ray tracing workload

### Stress Tests
- **Heavy Load Stress** - Continuous sqrt/sin/cos workload for sustained 95-100% CPU saturation
- **Instability Check Mode** - 4-phase rotating workload to detect marginal CPU instabilities and voltage droops
  - Phase 1: FP-heavy (sqrt/sin/cos)
  - Phase 2: Integer operations (bit manipulation)
  - Phase 3: Memory operations (pointer chasing simulation)
  - Phase 4: Mixed bandwidth workload
  - Periodic idle dips to detect voltage regulation issues

### Control Features
- Pause/Resume stress testing
- Real-time status display
- CPU info and metrics display

## System Requirements

- **OS**: Windows XP SP3, Windows Vista, or Windows 7
- **RAM**: 1GB minimum
- **.NET Framework**: 4.0 or higher

## Compatibility

This version is built on **.NET Framework 4.0**, which supports:
- ✓ Windows XP SP3
- ✓ Windows Vista
- ✓ Windows 7
- ✓ Windows 8+ (obviously)

## Building

### Prerequisites
- Visual Studio 2010+ or Visual Studio Build Tools
- .NET Framework 4.0 Target Pack

### Compile
```bash
msbuild XenoCPUUtility-Legacy.csproj /p:Configuration=Release
```

Or open in Visual Studio and build normally.

## Usage

1. Run `XenoCPUUtilityLegacy.exe`
2. Click a benchmark button to run performance tests
3. Click a stress test button to begin CPU stress testing
4. Use Pause/Resume to temporarily suspend testing
5. Click Stop Stress to end testing

Results appear in the text box with timestamps.

## Architecture

### Core Components

**BenchmarkEngine.cs**
- Single-thread benchmark using math operations
- Multi-thread benchmark with parallelization
- Normalized scoring system

**StressEngine.cs**
- Heavy Load mode: continuous FP workload
- Instability Check mode: rotating 4-phase workload
- Pause/Resume via ManualResetEvent
- Thread-safe operation

**PathTracerBenchmark.cs**
- Simple path tracing renderer (no dependencies)
- Simplified Vector3 math implementation
- Sphere and plane ray intersection
- Parallel pixel processing

**HardwareMetrics.cs**
- CPU load via Performance Counters
- CPU frequency via WMI
- CPU core/thread count

**MainForm.cs**
- Simple WinForms UI (no WebView2 dependency)
- Results display
- Benchmark/stress control

## Differences from Modern Version

1. **No WebView2** - Uses simple WinForms UI instead
2. **No RAMStress** - Removed memory stress module (can be added if needed)
3. **No WHEA monitoring** - Simplified hardware error detection
4. **Simplified graphics** - No chart rendering (results only in text)
5. **.NET 4.0 only** - No C# 8+ features, using older patterns

## Performance Notes

- Single-core benchmark targets ~10 seconds
- Multi-core benchmark targets ~6 seconds
- Path tracing is CPU-bound and scales with thread count
- Stress test utilizes all available cores by default
- Tests are deterministic (same seed for reproducibility)

## License

Ported from XenoCPUUtility by nkthebass
Legacy version created for historical OS support
