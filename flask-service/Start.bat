@echo off
REM ─────────────────────────────────────────────────────────────────────────
REM  Start.bat — Launch VTU PDF Extractor Flask service (Windows)
REM
REM  Usage: Double-click OR run from terminal
REM  Flask runs at: http://localhost:5050
REM
REM  First-time setup:
REM    pip install -r requirements-windows.txt
REM ─────────────────────────────────────────────────────────────────────────

echo [VTU Flask Service] Starting PDF extractor on port 5050...
echo [VTU Flask Service] Keep this window open alongside Visual Studio.
echo.

REM Navigate to the folder this .bat file lives in
cd /d "%~dp0"

REM Activate venv if it exists (Windows path uses Scripts not bin)
if exist "venv\Scripts\activate.bat" (
    call venv\Scripts\activate.bat
    echo [VTU Flask Service] venv activated.
) else (
    echo [WARNING] No venv found. Using system Python.
    echo [WARNING] Run: python -m venv venv and pip install -r requirements-windows.txt
)

REM Start Flask
python flask_app.py

REM If Python fails, pause so you can read the error
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Flask failed to start. Check above for the error.
    echo         Make sure you ran: pip install -r requirements-windows.txt
    pause
)
