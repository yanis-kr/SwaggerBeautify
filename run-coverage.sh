#!/bin/bash

# Test Coverage Script for WebApi Swagger Code
# Runs tests with coverage and generates HTML report

echo "🧪 Running tests with coverage..."

# Clean previous coverage results
COVERAGE_DIR="./coverage"
if [ -d "$COVERAGE_DIR" ]; then
    rm -rf "$COVERAGE_DIR"
    echo "✓ Cleaned previous coverage results"
fi

# Run tests with coverage
echo ""
echo "📊 Collecting coverage data..."
dotnet test ./tests/WebApi.Tests/WebApi.Tests.csproj \
    --collect:"XPlat Code Coverage" \
    --results-directory:"$COVERAGE_DIR/raw" \
    --logger:"console;verbosity=minimal"

if [ $? -ne 0 ]; then
    echo "❌ Tests failed!"
    exit 1
fi

# Find the coverage file
COVERAGE_FILE=$(find "$COVERAGE_DIR/raw" -name "coverage.cobertura.xml" -type f | head -n 1)

if [ -z "$COVERAGE_FILE" ]; then
    echo "❌ Coverage file not found!"
    exit 1
fi

echo "✓ Coverage data collected"

# Generate HTML report
echo ""
echo "📈 Generating HTML coverage report..."
reportgenerator \
    -reports:"$COVERAGE_FILE" \
    -targetdir:"$COVERAGE_DIR/html" \
    -reporttypes:"Html;HtmlSummary;Badges" \
    -assemblyfilters:"+WebApi" \
    -classfilters:"-System.*;-Microsoft.*"

if [ $? -ne 0 ]; then
    echo "❌ Report generation failed!"
    exit 1
fi

echo "✓ HTML report generated"

# Display summary
echo ""
echo "📋 Coverage Summary:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

REPORT_PATH="$COVERAGE_DIR/html/index.html"

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Open report in default browser
echo ""
echo "🌐 Opening coverage report in browser..."

# Detect OS and open browser accordingly
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    xdg-open "$REPORT_PATH" 2>/dev/null || sensible-browser "$REPORT_PATH" 2>/dev/null
elif [[ "$OSTYPE" == "darwin"* ]]; then
    open "$REPORT_PATH"
elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
    start "$REPORT_PATH"
fi

echo ""
echo "✅ Coverage report generated successfully!"
echo "📁 Report location: $(pwd)/$REPORT_PATH"

