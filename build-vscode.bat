@echo off
REM Build using VS Code's integrated build system

cd /d "%~dp0"

echo.
echo ======================================================
echo  Building with VS Code (dotnet + MSBuild)
echo ======================================================
echo.

REM First, try to find and use Visual Studio Build Tools or installed VS
set MSBUILD=

REM Check for Visual Studio 2022
if exist "%ProgramFiles%\Microsoft Visual Studio\2022" (
    for /d %%D in ("%ProgramFiles%\Microsoft Visual Studio\2022\*") do (
        if exist "%%D\MSBuild\Current\Bin\MSBuild.exe" (
            set "MSBUILD=%%D\MSBuild\Current\Bin\MSBuild.exe"
            goto found_msbuild
        )
    )
)

REM Check for Visual Studio 2019
if exist "%ProgramFiles%\Microsoft Visual Studio\2019" (
    for /d %%D in ("%ProgramFiles%\Microsoft Visual Studio\2019\*") do (
        if exist "%%D\MSBuild\Current\Bin\MSBuild.exe" (
            set "MSBUILD=%%D\MSBuild\Current\Bin\MSBuild.exe"
            goto found_msbuild
        )
    )
)

REM If MSBuild not found, fall back to dotnet CLI
if "%MSBUILD%"=="" (
    echo Using dotnet CLI (VS Code environment)
    echo.
    dotnet build XenoCPUUtility-Legacy.csproj -c Release -v minimal
    goto build_done
)

:found_msbuild
echo Found MSBuild: %MSBUILD%
echo.
"%MSBUILD%" XenoCPUUtility-Legacy.csproj /p:Configuration=Release /p:Platform=AnyCPU /v:minimal

:build_done
if %ERRORLEVEL% equ 0 (
    echo.
    echo ======================================================
    echo  Build SUCCESSFUL!
    echo ======================================================
    echo.
    echo Output: bin\Release\XenoCPUUtilityLegacy.exe
    echo.
    pause
) else (
    echo.
    echo ======================================================
    echo  Build FAILED
    echo ======================================================
    echo.
    echo If you see ".NET Framework 4.0" error:
    echo.
    echo  1. Install .NET Framework 4.0 Developer Pack:
    echo     https://aka.ms/msbuild/developerpacks
    echo.
    echo  2. Or install Visual Studio with .NET desktop dev:
    echo     https://visualstudio.microsoft.com/downloads/
    echo.
    pause
    exit /b 1
)
