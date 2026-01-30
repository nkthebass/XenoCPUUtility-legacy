@echo off
REM Quick build script - simpler version with MSBuild detection

cd /d "%~dp0"

REM Find MSBuild
set MSBUILD=

REM Try to find MSBuild in Visual Studio 2022
for /f "tokens=*" %%A in ('dir /b "%ProgramFiles(x86)%\Microsoft Visual Studio\2022" 2^>nul') do (
    if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2022\%%A\MSBuild\Current\Bin\MSBuild.exe" (
        set "MSBUILD=%ProgramFiles(x86)%\Microsoft Visual Studio\2022\%%A\MSBuild\Current\Bin\MSBuild.exe"
        goto found
    )
)

REM Try Visual Studio 2019
for /f "tokens=*" %%A in ('dir /b "%ProgramFiles(x86)%\Microsoft Visual Studio\2019" 2^>nul') do (
    if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\%%A\MSBuild\Current\Bin\MSBuild.exe" (
        set "MSBUILD=%ProgramFiles(x86)%\Microsoft Visual Studio\2019\%%A\MSBuild\Current\Bin\MSBuild.exe"
        goto found
    )
)

REM Try older Visual Studio installations
if exist "%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" set "MSBUILD=%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
if exist "%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe" set "MSBUILD=%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"

:found
if "%MSBUILD%"=="" (
    echo ERROR: MSBuild not found. Please install Visual Studio 2010 or later.
    pause
    exit /b 1
)

echo Using MSBuild: %MSBUILD%
echo Building XenoCPUUtility-Legacy...
echo.

"%MSBUILD%" XenoCPUUtility-Legacy.csproj /p:Configuration=Release /p:Platform=AnyCPU

if %ERRORLEVEL% equ 0 (
    echo.
    echo Build successful! Output: bin\Release\XenoCPUUtilityLegacy.exe
    echo.
    pause
) else (
    echo.
    echo Build failed. See errors above.
    echo.
    pause
)
