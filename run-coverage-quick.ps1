# Quick Coverage Script - Just open the last generated report

$reportPath = ".\coverage\html\index.html"

if (-not (Test-Path $reportPath)) {
    Write-Host "❌ No coverage report found!" -ForegroundColor Red
    Write-Host "💡 Run './run-coverage.ps1' first to generate a report" -ForegroundColor Yellow
    exit 1
}

Write-Host "🌐 Opening coverage report in Edge..." -ForegroundColor Cyan
$fullPath = Resolve-Path $reportPath
Start-Process "msedge.exe" $fullPath.Path

Write-Host "✅ Report opened!" -ForegroundColor Green

