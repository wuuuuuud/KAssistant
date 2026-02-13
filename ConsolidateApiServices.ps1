# PowerShell script to complete API service consolidation
# Run this script from the KAssistant project root directory

Write-Host "Starting API Service Consolidation..." -ForegroundColor Cyan
Write-Host ""

$servicesPath = ".\Services"
$examplesPath = ".\Examples"

# Step 1: Backup the original file (just in case)
Write-Host "Step 1: Creating backup..." -ForegroundColor Yellow
if (Test-Path "$servicesPath\KavitaApiService.cs") {
    Copy-Item "$servicesPath\KavitaApiService.cs" "$servicesPath\KavitaApiService.cs.backup"
    Write-Host "  ? Backup created: KavitaApiService.cs.backup" -ForegroundColor Green
}

# Step 2: Replace the old service with the new unified version
Write-Host ""
Write-Host "Step 2: Replacing with unified service..." -ForegroundColor Yellow
if (Test-Path "$servicesPath\KavitaApiService_New.cs") {
    Remove-Item "$servicesPath\KavitaApiService.cs" -Force -ErrorAction SilentlyContinue
    Rename-Item "$servicesPath\KavitaApiService_New.cs" "KavitaApiService.cs"
    Write-Host "  ? KavitaApiService.cs updated with unified version" -ForegroundColor Green
} else {
    Write-Host "  ? KavitaApiService_New.cs not found!" -ForegroundColor Red
    exit 1
}

# Step 3: Delete obsolete service files
Write-Host ""
Write-Host "Step 3: Removing obsolete files..." -ForegroundColor Yellow

$filesToDelete = @(
    "$servicesPath\OpenApiKavitaService.cs",
    "$servicesPath\KavitaOpenApiService.cs",
    "$examplesPath\OpenApiExamples.cs"
)

foreach ($file in $filesToDelete) {
    if (Test-Path $file) {
        Remove-Item $file -Force
        Write-Host "  ? Deleted: $file" -ForegroundColor Green
    } else {
        Write-Host "  - Not found: $file" -ForegroundColor Gray
    }
}

# Step 4: Clean and rebuild
Write-Host ""
Write-Host "Step 4: Cleaning and rebuilding solution..." -ForegroundColor Yellow

Write-Host "  Running: dotnet clean" -ForegroundColor Gray
dotnet clean 2>&1 | Out-Null

Write-Host "  Running: dotnet build" -ForegroundColor Gray
$buildOutput = dotnet build 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Build successful!" -ForegroundColor Green
} else {
    Write-Host "  ? Build failed. Output:" -ForegroundColor Red
    Write-Host $buildOutput -ForegroundColor Red
    Write-Host ""
    Write-Host "You may need to manually resolve build errors." -ForegroundColor Yellow
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "API Service Consolidation Complete!" -ForegroundColor Cyan  
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Summary:" -ForegroundColor White
Write-Host "  ? Unified service: Services/KavitaApiService.cs" -ForegroundColor White
Write-Host "  ? Obsolete files removed: 3" -ForegroundColor White
Write-Host "  ? Backup created: KavitaApiService.cs.backup" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor White
Write-Host "  1. Test the application" -ForegroundColor Gray
Write-Host "  2. If everything works, delete the backup file" -ForegroundColor Gray
Write-Host "  3. Commit your changes" -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
