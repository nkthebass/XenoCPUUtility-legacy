@echo off
REM XenoCPUUtility Legacy 64-bit Build Script
REM Builds the legacy .NET Framework 4.0 version for Windows 64-bit

setlocal enabledelayedexpansion

echo.
echo ======================================================================
echo  XenoCPUUtility Legacy 64-bit Build Script
echo ======================================================================
echo.

REM Get the directory where this script is located
set SCRIPT_DIR=%~dp0
cd /d "%SCRIPT_DIR%"

REM Configuration
set PROJECT_NAME=XenoCPUUtility-Legacy
set PROJECT_FILE=%PROJECT_NAME%.csproj
set CONFIG=Release
set PLATFORM=x64
set OUTPUT_DIR=bin\%CONFIG%\x64\

REM Check if project file exists
if not exist "%PROJECT_FILE%" (
    echo ERROR: Project file not found: %PROJECT_FILE%
    echo Current directory: %cd%
    echo.
    pause
    exit /b 1
)

echo Project found: %PROJECT_FILE%
echo Configuration: %CONFIG%
echo Output directory: %OUTPUT_DIR%
echo.

REM Find MSBuild
set MSBUILD_PATH=
for /f "tokens=*" %%A in ('where msbuild.exe 2^>nul') do set MSBUILD_PATH=%%A

if "%MSBUILD_PATH%"=="" (
    echo Searching for MSBuild in Visual Studio installation paths...
    if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2022\*\MSBuild\Current\Bin\MSBuild.exe" (
        for /d %%D in ("%ProgramFiles(x86)%\Microsoft Visual Studio\2022\*") do (
            if exist "%%D\MSBuild\Current\Bin\MSBuild.exe" (
                set "MSBUILD_PATH=%%D\MSBuild\Current\Bin\MSBuild.exe"
                goto found_msbuild
            )
        )
    )
    if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\*\MSBuild\Current\Bin\MSBuild.exe" (
        for /d %%D in ("%ProgramFiles(x86)%\Microsoft Visual Studio\2019\*") do (
            if exist "%%D\MSBuild\Current\Bin\MSBuild.exe" (
                set "MSBUILD_PATH=%%D\MSBuild\Current\Bin\MSBuild.exe"
                goto found_msbuild
            )
        )
    )
    if exist "%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" (
        set "MSBUILD_PATH=%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
        goto found_msbuild
    )
    if exist "%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe" (
        set "MSBUILD_PATH=%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
        goto found_msbuild
    )
    echo ERROR: MSBuild not found!
    echo Please install Visual Studio 2010 or later, or the MSBuild tools.
    echo.
    pause
    exit /b 1
)

:found_msbuild
echo Found MSBuild: %MSBUILD_PATH%
echo.

REM Clean previous build
echo Cleaning previous build...
if exist "%OUTPUT_DIR%" (
    rmdir /s /q "%OUTPUT_DIR%"
)
echo.

REM Build the project
echo Building %PROJECT_NAME% (%CONFIG% x64)...
echo.
"%MSBUILD_PATH%" "%PROJECT_FILE%" /p:Configuration=%CONFIG% /p:Platform=x64 /p:DebugType=pdbonly

if %ERRORLEVEL% neq 0 (
    echo.
    echo ERROR: Build failed with exit code %ERRORLEVEL%
    echo.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ======================================================================
echo  Build Successful!
echo ======================================================================
echo.
echo Output: %cd%\%OUTPUT_DIR%
echo.

REM Check if executable exists
if exist "%OUTPUT_DIR%%PROJECT_NAME%.exe" (
    echo Executable created: %OUTPUT_DIR%%PROJECT_NAME%.exe
    echo.
    echo To run the program, execute:
    echo   %OUTPUT_DIR%%PROJECT_NAME%.exe
    echo.
) else (
    echo WARNING: Executable not found at expected location!
)

echo.
pause
exit /b 0
