# Setup Instructions for Building XenoCPUUtility Legacy

## Current Situation

Your system has:
- ✅ .NET SDK 8.0
- ❌ Visual Studio or MSBuild
- ❌ .NET Framework 4.0 Developer Pack

To build XenoCPUUtility-Legacy, you have two options:

---

## Option 1: Install .NET Framework 4.0 Developer Pack (Easiest)

### For .NET 8 SDK Users

If you have .NET 8 SDK installed and want to build with `dotnet` command:

1. **Download and install** the .NET Framework 4.0 Developer Pack:
   - Download from: https://aka.ms/msbuild/developerpacks
   - Or directly: https://www.microsoft.com/en-us/download/details.aspx?id=17718

2. **Run the installer** and follow prompts

3. **Build the project**:
   ```batch
   cd XenoCPUUtility-Legacy
   build-dotnet.bat
   ```

**Estimated time**: 5 minutes

---

## Option 2: Install Visual Studio Community (Recommended for Development)

### Full IDE with Integrated Building

1. **Download** Visual Studio Community (free):
   - https://visualstudio.microsoft.com/downloads/
   - Select "Community" edition

2. **During installation**:
   - Choose ".NET desktop development"
   - Make sure to include .NET Framework 4.0 targeting pack

3. **Open the solution**:
   ```batch
   XenoCPUUtility-Legacy.sln
   ```

4. **Build in Visual Studio**:
   - Press Ctrl+Shift+B
   - Or right-click project → Build

5. **Or use batch script**:
   ```batch
   build.bat
   ```

**Estimated time**: 20-30 minutes download + install

---

## Option 3: Install Only MSBuild Tools

### Lightweight Alternative to Visual Studio

1. **Download** Visual Studio Build Tools:
   - https://visualstudio.microsoft.com/downloads/
   - Scroll down to "Tools for Visual Studio"
   - Select "Build Tools for Visual Studio"

2. **During installation**:
   - Select "MSBuild tools"
   - Select ".NET Framework 4.0 targeting pack"

3. **Build with batch script**:
   ```batch
   build.bat
   ```

**Estimated time**: 10-15 minutes

---

## Quick Test: Which Option Do You Have?

Run this command to check:

```batch
REM Check for Visual Studio
dir "%ProgramFiles(x86)%\Microsoft Visual Studio" 

REM Check for MSBuild
dir "%ProgramFiles(x86)%\MSBuild"

REM Check for dotnet SDK
dotnet --version
```

---

## After Installation

Once you have one of the above options installed:

### Using batch files (Easy):
```batch
cd XenoCPUUtility-Legacy

REM Choose one based on what you installed:
build.bat              REM (for MSBuild)
build-dotnet.bat       REM (for dotnet CLI)
build-quick.bat        REM (for MSBuild - simpler)

REM Then run:
run.bat
```

### Using Visual Studio (Most GUI-friendly):
1. Open XenoCPUUtility-Legacy.sln
2. Press Ctrl+Shift+B
3. Press Ctrl+F5 to run

---

## Batch Files Available

| File | Requirements | Best For |
|------|--------------|----------|
| `build.bat` | Visual Studio 2010+ or MSBuild | Comprehensive builds |
| `build-quick.bat` | Visual Studio 2010+ or MSBuild | Quick local builds |
| `build-dotnet.bat` | .NET 8 SDK + .NET 4.0 pack | Modern SDK users |
| `run.bat` | Compiled exe | Running built program |

---

## Full Build Process

Once you have the requirements installed:

### Step 1: Open Command Prompt
```batch
cd C:\Users\noahc\Downloads\XenoCPUUtility-1.9.4\XenoCPUUtility-1.9.4\XenoCPUUtility-Legacy
```

### Step 2: Build
```batch
build-dotnet.bat
REM or
build-quick.bat
REM or use Visual Studio IDE
```

### Step 3: Run
```batch
run.bat
```

---

## Recommended Installation Path

For fastest setup:

1. **Install .NET Framework 4.0 Developer Pack** (5 min)
   - Download: https://aka.ms/msbuild/developerpacks
   - Follow installer

2. **Build with batch file** (30 sec):
   ```batch
   build-dotnet.bat
   ```

3. **Run**:
   ```batch
   run.bat
   ```

**Total time**: ~10 minutes

---

## Troubleshooting

**"MSBuild not found"**
- Need: Visual Studio 2010+ OR MSBuild Build Tools
- Install: https://visualstudio.microsoft.com/downloads/

**".NET Framework 4.0 was not found"**
- Need: .NET Framework 4.0 Developer Pack
- Install: https://aka.ms/msbuild/developerpacks

**"dotnet command not found"**
- Need: .NET SDK (any version)
- Have: dotnet 8.0.416 ✅

**"XenoCPUUtility-Legacy.csproj not found"**
- Make sure you're in the right directory:
  ```batch
  cd XenoCPUUtility-Legacy
  dir *.csproj
  ```

---

## Next Steps

1. Pick an installation option above
2. Install the required tools
3. Run one of the batch files
4. Executable will be at: `bin\Release\XenoCPUUtilityLegacy.exe`

Need help? Check the error message in the batch script output!
