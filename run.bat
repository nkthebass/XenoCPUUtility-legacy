@echo off
REM Run the compiled XenoCPUUtility-Legacy application

cd /d "%~dp0"

set EXE_PATH=bin\Release\XenoCPUUtilityLegacy.exe

if not exist "%EXE_PATH%" (
    echo.
    echo ERROR: Executable not found at: %EXE_PATH%
    echo.
    echo Please build the project first using build.bat or build-quick.bat
    echo.
    pause
    exit /b 1
)

echo.
echo Starting XenoCPUUtility-Legacy...
echo.

"%EXE_PATH%"

pause
