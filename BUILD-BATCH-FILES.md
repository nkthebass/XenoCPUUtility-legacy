# Using Batch Files to Build and Run

Created three batch files to make building and running the legacy version easier:

## Files Created

### 1. **build.bat** (Recommended)
Comprehensive build script with:
- Automatic MSBuild detection (searches for Visual Studio installations)
- Detailed error reporting
- Cleans previous builds
- Displays output location
- Pauses at end to see results

**How to use:**
```batch
Double-click build.bat
OR
From command line: build.bat
```

### 2. **build-quick.bat** (Simplest)
Quick build script - minimal overhead:
- Direct MSBuild call
- Fast execution
- Good for experienced users

**How to use:**
```batch
Double-click build-quick.bat
OR
From command line: build-quick.bat
```

### 3. **run.bat** (After Building)
Runs the compiled executable:
- Checks if executable exists
- Provides clear error messages
- Automatically pauses to show output

**How to use:**
```batch
Double-click run.bat
OR
From command line: run.bat
```

## Quick Start Steps

1. **First time only** - Build the project:
   - Double-click `build.bat`
   - Wait for build to complete
   - Press any key to close

2. **Run the program**:
   - Double-click `run.bat`
   - Application window opens
   - Click buttons to run benchmarks/stress tests

3. **Rebuild after changes**:
   - Double-click `build.bat` again
   - Then run with `run.bat`

## From Command Line

```bash
# Build (using the quick build)
cd XenoCPUUtility-Legacy
build-quick.bat

# Or with detailed output
build.bat

# Run
run.bat
```

## Troubleshooting

**"Build failed" message:**
- Ensure you have Visual Studio 2010+ or MSBuild tools installed
- Check that XenoCPUUtility-Legacy.csproj exists in the directory
- Try running build.bat from the project directory

**"Executable not found" when running:**
- Build hasn't completed yet, or failed
- Run build.bat first to compile the project
- Check that the build completed successfully

**MSBuild not found:**
- Install Visual Studio Community (free) from microsoft.com
- Or install .NET Framework 4.0 SDK

## Environment Requirements

- **For Building**: Visual Studio 2010+ OR MSBuild tools + .NET Framework 4.0 SDK
- **For Running**: .NET Framework 4.0 or higher

## Advanced: Build Options

You can customize the build by editing build.bat:

```batch
REM Change output configuration (Release or Debug)
set CONFIG=Release

REM Change platform (AnyCPU, x86, or x64)
set PLATFORM=AnyCPU
```

## What Happens During Build

1. Searches for MSBuild executable
2. Cleans previous build output
3. Compiles C# source files
4. Links .NET assemblies
5. Produces XenoCPUUtilityLegacy.exe in bin\Release\

Total build time: 5-15 seconds (depending on system)

## Output Location

After successful build:
```
XenoCPUUtility-Legacy/
└── bin/
    └── Release/
        ├── XenoCPUUtilityLegacy.exe     ← The executable
        ├── XenoCPUUtilityLegacy.pdb     ← Debug symbols
        └── [other compiled files]
```

## Tips

✅ **Use build.bat** - Most reliable with auto-detection
✅ **Keep .bat files in project root** - They use relative paths
✅ **Check console output** - Errors are displayed clearly
✅ **Run as Administrator** - If you get permission errors
✅ **Close Visual Studio** - Before building with .bat files (avoids lock issues)

## One-Line Build & Run

Create a new batch file with:
```batch
@echo off
call build.bat
if %ERRORLEVEL% equ 0 call run.bat
pause
```

Save as `build-and-run.bat` for single-click build & execute.
