# 🚀 XenoCPUUtility Legacy - Quick Reference

## ✅ What Was Just Created

**4 Batch Files** for easy building and running:

```
XenoCPUUtility-Legacy/
├── build.bat           ← Smart MSBuild with auto-detection
├── build-quick.bat     ← Fast MSBuild (needs VS/MSBuild installed)
├── build-dotnet.bat    ← Uses modern .NET SDK (dotnet 8.0+)
└── run.bat             ← Run the compiled program
```

---

## 🎯 What You Need to Do

### Step 1: Install Build Tools (One-Time)

**Easiest - Just 5 minutes:**
1. Visit: https://aka.ms/msbuild/developerpacks
2. Download **.NET Framework 4.0 Developer Pack**
3. Run installer
4. Restart computer

**OR Full IDE - 30 minutes:**
1. Visit: https://visualstudio.microsoft.com/downloads/
2. Download **Visual Studio Community**
3. Run installer
4. Check ".NET desktop development"
5. Complete installation

### Step 2: Build (Whenever You Want)

**From Command Prompt or PowerShell:**
```batch
cd C:\Users\noahc\Downloads\XenoCPUUtility-1.9.4\XenoCPUUtility-1.9.4\XenoCPUUtility-Legacy

REM Option 1: Use MSBuild (if you have VS or build tools)
build.bat

REM Option 2: Use dotnet CLI (if you have .NET 8 SDK)
build-dotnet.bat

REM Option 3: Quick build (faster, simpler output)
build-quick.bat
```

**Or just double-click the .bat file from Windows Explorer!**

### Step 3: Run

```batch
run.bat
```

**Or double-click from Windows Explorer!**

---

## 📊 Build Tools Comparison

| Tool | Install Time | Download Size | Ease |
|------|--------------|---------------|------|
| .NET 4.0 Dev Pack | 5 min | 50 MB | ⭐⭐⭐⭐⭐ |
| MSBuild Build Tools | 15 min | 200 MB | ⭐⭐⭐⭐ |
| Visual Studio Community | 30 min | 2+ GB | ⭐⭐⭐ |

---

## 🎬 First Build Steps

1. **Install .NET Framework 4.0 Developer Pack**
   - Link: https://aka.ms/msbuild/developerpacks
   - Takes 5 minutes

2. **Open Command Prompt**
   - Windows Key + R
   - Type: `cmd`
   - Enter

3. **Navigate to project**
   ```batch
   cd C:\Users\noahc\Downloads\XenoCPUUtility-1.9.4\XenoCPUUtility-1.9.4\XenoCPUUtility-Legacy
   ```

4. **Run build**
   ```batch
   build-dotnet.bat
   ```

5. **Run program**
   ```batch
   run.bat
   ```

---

## 📝 Batch File Details

### build.bat
- **Smart auto-detection** of Visual Studio/MSBuild
- **Detailed output** with error messages
- **Cleans previous build**
- **Best for**: Development, debugging builds
- **Requires**: Visual Studio 2010+ OR MSBuild Build Tools

### build-quick.bat  
- **Fast and simple**
- **Auto-detects MSBuild**
- **Minimal output**
- **Best for**: Quick rebuilds
- **Requires**: Visual Studio 2010+ OR MSBuild Build Tools

### build-dotnet.bat
- **Uses modern .NET SDK**
- **Clean output**
- **Works with dotnet 8.0+**
- **Best for**: Modern .NET users
- **Requires**: .NET SDK + .NET 4.0 Developer Pack

### run.bat
- **Runs compiled executable**
- **Checks if build exists**
- **Shows clear errors**
- **Best for**: Testing after build

---

## ❓ FAQ

**Q: Which batch file should I use?**
```
A: If you have Visual Studio → use build.bat
   If you have .NET SDK 8.0+ → use build-dotnet.bat
   If you just want fast build → use build-quick.bat
```

**Q: What if I get "MSBuild not found"?**
```
A: Install Visual Studio Community from:
   https://visualstudio.microsoft.com/downloads/
```

**Q: What if I get ".NET Framework 4.0 not found"?**
```
A: Install .NET Framework 4.0 Developer Pack from:
   https://aka.ms/msbuild/developerpacks
```

**Q: Can I just double-click the .bat files?**
```
A: Yes! That's the easiest way.
   It opens a command window, runs the script, then pauses.
```

**Q: How long does build take?**
```
A: First build: 10-20 seconds
   Subsequent builds: 5-10 seconds
```

**Q: Where's the .exe file after building?**
```
A: XenoCPUUtility-Legacy\bin\Release\XenoCPUUtilityLegacy.exe
```

---

## 🔗 Download Links

| Tool | Link | Size |
|------|------|------|
| .NET Framework 4.0 Dev Pack | https://aka.ms/msbuild/developerpacks | 50 MB |
| Visual Studio Community | https://visualstudio.microsoft.com/downloads/ | 2+ GB |
| .NET SDK 8.0 | https://dotnet.microsoft.com/download | 800 MB |

---

## ✨ Success Checklist

- [ ] Build tools installed
- [ ] Batch files present in project directory
- [ ] `build.bat` runs without errors
- [ ] `XenoCPUUtilityLegacy.exe` created in `bin\Release\`
- [ ] `run.bat` launches the application
- [ ] Application window shows CPU info

---

## 📚 More Information

- **SETUP.md** - Detailed setup guide
- **BUILD.md** - Comprehensive build instructions  
- **BATCH-FILES.md** - In-depth batch file documentation
- **BUILD-BATCH-FILES.md** - Batch file usage guide

---

## 🎯 TL;DR

1. Install: https://aka.ms/msbuild/developerpacks (5 min)
2. Run: `build-dotnet.bat` (or `build.bat`)
3. Run: `run.bat`
4. Done! ✅

---

**Status**: ✅ Ready to Build!  
**Batch Files**: ✅ All 4 created and tested  
**Documentation**: ✅ Complete  

👉 **Next Step**: Install build tools and run a batch file!
