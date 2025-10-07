# Test Coverage Script for WebApi Swagger Code
# Runs tests with coverage and generates HTML report

Write-Host "Running tests with coverage..." -ForegroundColor Cyan

# Clean previous coverage results
$coverageDir = ".\coverage"
if (Test-Path $coverageDir) {
    Remove-Item $coverageDir -Recurse -Force
    Write-Host "Cleaned previous coverage results" -ForegroundColor Green
}

# Run tests with coverage
Write-Host "`nCollecting coverage data..." -ForegroundColor Cyan
dotnet test .\src\WebApi.Tests\WebApi.Tests.csproj `
    --collect:"XPlat Code Coverage" `
    --results-directory:"$coverageDir\raw" `
    --logger:"console;verbosity=minimal"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Tests failed!" -ForegroundColor Red
    exit 1
}

# Find the coverage file
$coverageFile = Get-ChildItem -Path "$coverageDir\raw" -Recurse -Filter "coverage.cobertura.xml" | Select-Object -First 1

if (-not $coverageFile) {
    Write-Host "Coverage file not found!" -ForegroundColor Red
    exit 1
}

Write-Host "Coverage data collected" -ForegroundColor Green

# Generate HTML report
Write-Host "`nGenerating HTML coverage report..." -ForegroundColor Cyan
reportgenerator `
    -reports:"$($coverageFile.FullName)" `
    -targetdir:"$coverageDir\html" `
    -reporttypes:"Html;HtmlSummary;Badges" `
    -assemblyfilters:"+WebApi" `
    -classfilters:"-System.*;-Microsoft.*"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Report generation failed!" -ForegroundColor Red
    exit 1
}

Write-Host "HTML report generated" -ForegroundColor Green

# Display summary
Write-Host "`nCoverage Summary:" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Gray

# Parse summary from report
$summaryFile = "$coverageDir\html\index.html"
if (Test-Path $summaryFile) {
    $summary = Get-Content $summaryFile -Raw
    
    # Extract coverage percentage (simplified)
    if ($summary -match 'Line coverage.*?(\d+\.?\d*)%') {
        $lineCoverage = $matches[1]
        Write-Host "Line Coverage:   $lineCoverage%" -ForegroundColor White
    }
    
    if ($summary -match 'Branch coverage.*?(\d+\.?\d*)%') {
        $branchCoverage = $matches[1]
        Write-Host "Branch Coverage: $branchCoverage%" -ForegroundColor White
    }
}

Write-Host "========================================" -ForegroundColor Gray

# Open report in Edge
Write-Host "`nOpening coverage report in Edge..." -ForegroundColor Cyan
$reportPath = Resolve-Path "$coverageDir\html\index.html"
Start-Process "msedge.exe" $reportPath.Path

Write-Host "`nCoverage report generated successfully!" -ForegroundColor Green
Write-Host "Report location: $reportPath" -ForegroundColor White
