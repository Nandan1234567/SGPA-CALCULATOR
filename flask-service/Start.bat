@echo off
REM ─────────────────────────────────────────────────────────────────────────
REM  start.bat — Launch VTU PDF Extractor Flask service on Windows
REM
REM  HOW TO USE:
REM    1. Double-click this file  OR  run it from a terminal
REM    2. Keep the window open while your ASP.NET app is running
REM    3. Flask will be available at http://localhost:5050
REM
REM  FIRST-TIME SETUP:
REM    pip install -r requirements.txt
REM ─────────────────────────────────────────────────────────────────────────

echo [VTU Flask Service] Starting PDF extractor on port 5050...
echo [VTU Flask Service] Keep this window open alongside Visual Studio.
echo.

REM Navigate to the folder this .bat file lives in
cd /d "%~dp0"

REM Start Flask
python flask_app.py

REM If Python fails (not found, wrong env), pause so you can read the error
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Flask failed to start. Make sure Python is in PATH
    echo         and you have run:  pip install -r requirements.txt
    pause
)