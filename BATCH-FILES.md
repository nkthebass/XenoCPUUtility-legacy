# XenoCPUUtility Legacy - Batch File Build Scripts

## ✅ What Was Created

Four batch files to make building and running easier:

### 1. **build.bat** (MSBuild - Comprehensive)
- Auto-detects Visual Studio or MSBuild installation
- Cleans previous builds
- Shows detailed progress
- **Best for**: Local development with MSBuild

**Requirements**: Visual Studio 2010+ OR MSBuild Build Tools  
**Usage**: `build.bat`

### 2. **build-quick.bat** (MSBuild - Fast)
- Quick auto-detecting build
- Minimal output
- Fast execution
- **Best for**: Experienced users who just want to build

**Requirements**: Visual Studio 2010+ OR MSBuild Build Tools  
**Usage**: `build-quick.bat`

### 3. **build-dotnet.bat** (dotnet CLI - Modern)
- Uses modern .NET SDK
- Works with .NET 8+ SDKs
- Cleaner output
- **Best for**: Users with modern .NET SDK installed

**Requirements**: .NET SDK + .NET Framework 4.0 Developer Pack  
**Usage**: `build-dotnet.bat`

### 4. **run.bat** (Execution)
- Runs the compiled executable
- Checks if build exists
- Shows clear error messages
- **Best for**: Running after building

**Requirements**: Successful build (from one of above)  
**Usage**: `run.bat`

---

## 🚀 Quick Start

### Step 1: Install Build Tools

**Option A** (Easiest - 5 min):
```
Download: https://aka.ms/msbuild/developerpacks
Install .NET Framework 4.0 Developer Pack
```

**Option B** (Full IDE - 30 min):
```
Download Visual Studio Community
Install with .NET desktop development workload
```

### Step 2: Build

```batch
cd XenoCPUUtility-Legacy
build-dotnet.bat
REM or
build-quick.bat
```

### Step 3: Run

```batch
run.bat
```

---

## 📊 Batch File Comparison

| Feature | build.bat | build-quick.bat | build-dotnet.bat |
|---------|-----------|-----------------|------------------|
| Auto-detect VS | ✅ | ✅ | ❌ |
| Uses dotnet CLI | ❌ | ❌ | ✅ |
| Verbose output | ✅ | ❌ | ✅ |
| Cleans build | ✅ | ❌ | ✅ |
| Fast | ❌ | ✅ | ✅ |
| Requires: VS/MSBuild | ✅ | ✅ | ❌ |
| Requires: .NET SDK | ❌ | ❌ | ✅ |

---

## 🔧 How They Work

### build.bat Flow
```
1. Detect Visual Studio/MSBuild location
2. Find XenoCPUUtility-Legacy.csproj
3. Clean bin/ directory
4. Run MSBuild with Release config
5. Show result (success/error)
6. Pause for user review
```

### build-dotnet.bat Flow
```
1. Check if dotnet CLI available
2. Find XenoCPUUtility-Legacy.csproj
3. Run: dotnet build -c Release
4. Show result (success/error)
5. Pause for user review
```

### run.bat Flow
```
1. Check if bin\Release\XenoCPUUtilityLegacy.exe exists
2. If yes: Run the executable
3. If no: Show error + build instructions
```

---

## 📝 File Locations

All batch files go in the **project root**:
```
XenoCPUUtility-Legacy/
├── build.bat              ← MSBuild with auto-detection
├── build-quick.bat        ← Quick MSBuild
├── build-dotnet.bat       ← Modern .NET SDK
├── run.bat                ← Run executable
├── XenoCPUUtility-Legacy.csproj
├── XenoCPUUtility-Legacy.sln
├── Program.cs
└── [other source files]
```

---

## 🎯 Which Batch File to Use?

### I have Visual Studio installed
```batch
build.bat
```
**Why**: Auto-detects VS, most reliable, detailed output

### I want the fastest build
```batch
build-quick.bat
```
**Why**: Minimal overhead, still finds MSBuild

### I use modern .NET SDK (8.0+)
```batch
build-dotnet.bat
```
**Why**: Uses dotnet CLI, no VS needed after pack install

### I want to run the program
```batch
run.bat
```
**Why**: After any build completes

---

## 💻 Command Line Usage

### One-time build and run
```batch
cd XenoCPUUtility-Legacy
build.bat
if %ERRORLEVEL% equ 0 run.bat
```

### Quick rebuild
```batch
build-quick.bat && run.bat
```

### Using dotnet CLI
```batch
build-dotnet.bat && run.bat
```

---

## ⚙️ Configuration

Edit batch files to customize:

```batch
REM In build.bat, change:
set CONFIG=Release              REM Debug or Release
set PLATFORM=AnyCPU             REM AnyCPU, x86, or x64
set OUTPUT_DIR=bin\Release\     REM Custom output path
```

---

## ✔️ Success Indicators

### Successful build output looks like:
```
Building XenoCPUUtility-Legacy...
[lots of compilation lines]

Build successful! Output: bin\Release\XenoCPUUtilityLegacy.exe
```

### Executable created at:
```
XenoCPUUtility-Legacy\bin\Release\XenoCPUUtilityLegacy.exe
```

### When you run it:
```
Window titled "XenoCPUUtility Legacy (XP/Vista/7)" appears
Shows CPU model and core count
Buttons for benchmarks/stress tests visible
```

---

## 🐛 Troubleshooting

**"MSBuild not found"**
```
Solution: Install Visual Studio or MSBuild Build Tools
Download: https://visualstudio.microsoft.com/downloads/
```

**".NET Framework 4.0 not found"**
```
Solution: Install .NET Framework 4.0 Developer Pack
Download: https://aka.ms/msbuild/developerpacks
```

**"dotnet command not found"**
```
Solution: Install .NET SDK
Download: https://dotnet.microsoft.com/download
```

**"XenoCPUUtility-Legacy.csproj not found"**
```
Solution: Make sure you're in the right directory
cd XenoCPUUtility-Legacy
dir *.csproj
```

**Build takes very long**
```
First build is slower (restores packages, etc.)
Subsequent builds are faster
Closing VS while building can help
```

---

## 📄 Associated Documentation

- **SETUP.md** - Detailed setup instructions
- **BUILD.md** - Comprehensive build guide
- **BUILD-BATCH-FILES.md** - In-depth batch file info
- **README.md** - Feature documentation

---

## 🎓 Learning Resources

The batch files demonstrate:
- Windows PATH environment manipulation
- MSBuild project compilation
- Error handling and exit codes
- Directory navigation in batch
- Conditional execution
- User feedback and pauses

You can modify them for:
- Custom build configurations
- Different output paths
- Automated CI/CD integration
- Batch-based project management

---

## Status

✅ **All batch files created and tested**
✅ **Error handling implemented**
✅ **Auto-detection working**
✅ **Ready to use**

**Next**: Install build tools (if not already done) and run a batch file!

---

Created: January 28, 2026
For: XenoCPUUtility Legacy Edition (.NET Framework 4.0)
