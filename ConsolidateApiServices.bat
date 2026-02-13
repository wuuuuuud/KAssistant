@echo off
REM Batch script to complete API service consolidation
REM Run this from the KAssistant project root directory

echo.
echo ========================================
echo API Service Consolidation Script
echo ========================================
echo.

cd /d "%~dp0KAssistant"

REM Step 1: Backup
echo Step 1: Creating backup...
if exist "Services\KavitaApiService.cs" (
    copy "Services\KavitaApiService.cs" "Services\KavitaApiService.cs.backup" >nul
    echo   [OK] Backup created
)

REM Step 2: Replace old file with new unified version
echo.
echo Step 2: Replacing with unified service...
if exist "Services\KavitaApiService_New.cs" (
    del "Services\KavitaApiService.cs" 2>nul
    ren "Services\KavitaApiService_New.cs" "KavitaApiService.cs"
    echo   [OK] KavitaApiService.cs updated
) else (
    echo   [ERROR] KavitaApiService_New.cs not found!
    pause
    exit /b 1
)

REM Step 3: Delete obsolete files
echo.
echo Step 3: Removing obsolete files...
if exist "Services\OpenApiKavitaService.cs" (
    del "Services\OpenApiKavitaService.cs"
    echo   [OK] Deleted OpenApiKavitaService.cs
)
if exist "Services\KavitaOpenApiService.cs" (
    del "Services\KavitaOpenApiService.cs"
    echo   [OK] Deleted KavitaOpenApiService.cs
)
if exist "Examples\OpenApiExamples.cs" (
    del "Examples\OpenApiExamples.cs"
    echo   [OK] Deleted OpenApiExamples.cs
)

REM Step 4: Clean and rebuild
echo.
echo Step 4: Cleaning and rebuilding...
echo   Running: dotnet clean
dotnet clean >nul 2>&1

echo   Running: dotnet build
dotnet build >nul 2>&1

if %ERRORLEVEL% EQU 0 (
    echo   [OK] Build successful!
) else (
    echo   [ERROR] Build failed!
    echo   Run 'dotnet build' manually to see errors
)

REM Summary
echo.
echo ========================================
echo Consolidation Complete!
echo ========================================
echo.
echo Summary:
echo   - Unified service: Services/KavitaApiService.cs
echo   - Obsolete files removed: 3
echo   - Backup: KavitaApiService.cs.backup
echo.
echo Next steps:
echo   1. Test the application
echo   2. Delete backup if everything works
echo   3. Commit your changes
echo.
pause
