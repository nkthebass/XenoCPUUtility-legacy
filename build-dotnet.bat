@echo off
REM Build using dotnet CLI (works with .NET SDK + VS Code)

cd /d "%~dp0"

echo Checking for dotnet CLI...
dotnet --version >nul 2>&1

if %ERRORLEVEL% neq 0 (
    echo ERROR: dotnet CLI not found. Please install .NET SDK.
    echo.
    echo Download from: https://dotnet.microsoft.com/download
    echo.
    pause
    exit /b 1
)

echo.
echo Building XenoCPUUtility-Legacy with dotnet CLI...
echo.

dotnet build XenoCPUUtility-Legacy.csproj -c Release

if %ERRORLEVEL% equ 0 (
    echo.
    echo ===============================================
    echo  Build successful!
    echo ===============================================
    echo.
    echo Output: bin\Release\XenoCPUUtilityLegacy.exe
    echo.
    pause
) else (
    echo.
    echo Build failed. Trying to diagnose...
    echo.
    echo Common issue: Missing .NET Framework 4.0 targeting pack
    echo.
    echo SOLUTION: Install .NET Framework 4.0 Developer Pack
    echo   Download: https://aka.ms/msbuild/developerpacks
    echo.
    pause
    exit /b 1
)
