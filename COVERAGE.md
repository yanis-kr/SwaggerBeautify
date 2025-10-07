# Test Coverage Guide

This project includes automated test coverage reporting for the Swagger/OpenAPI configuration code.

## Prerequisites

- ✅ .NET SDK (already installed)
- ✅ `coverlet.collector` NuGet package (already added to test project)
- ✅ `dotnet-reportgenerator-globaltool` (already installed globally)

## Quick Start

### Windows (PowerShell)

```powershell
# Run tests with coverage and open report in Edge
.\run-coverage.ps1

# Just open the last generated report
.\run-coverage-quick.ps1
```

### Linux / macOS (Bash)

```bash
# Run tests with coverage and open report in default browser
./run-coverage.sh
```

## What the Scripts Do

### `run-coverage.ps1` / `run-coverage.sh`

1. **Cleans** previous coverage results
2. **Runs** all unit tests with code coverage collection
3. **Generates** HTML coverage report with:
   - Line-by-line coverage visualization
   - Branch coverage metrics
   - Coverage badges
   - Detailed class and method breakdowns
4. **Opens** the report in Edge (Windows) or default browser (Linux/macOS)

### `run-coverage-quick.ps1`

- Quickly opens the last generated coverage report without re-running tests
- Useful for reviewing results without waiting for test execution

## Coverage Report Contents

The generated HTML report includes:

### Summary Dashboard

- **Overall Coverage Percentage** - Total lines covered
- **Branch Coverage** - Conditional branch coverage
- **Assembly Breakdown** - Coverage per project
- **Risk Areas** - Classes/methods with low coverage

### Detailed Views

- **Class Coverage** - Per-class coverage metrics
- **Method Coverage** - Individual method coverage
- **Line-by-Line** - Visual representation of covered/uncovered lines
- **Coverage Trends** - Historical coverage comparison (with multiple runs)

### Visual Indicators

- 🟢 **Green** - Well covered (>80%)
- 🟡 **Yellow** - Partially covered (50-80%)
- 🔴 **Red** - Low coverage (<50%)

## Coverage Report Location

After running the script:

```
📁 coverage/
├── raw/                    # Raw coverage data
│   └── {guid}/
│       └── coverage.cobertura.xml
└── html/                   # Generated HTML report
    ├── index.html          # Main report (opens automatically)
    ├── summary.html        # Coverage summary
    ├── badge_*.svg         # Coverage badges
    └── [class files]       # Detailed coverage per class
```

## Understanding Coverage Metrics

### Line Coverage

- **What it measures**: Percentage of code lines executed during tests
- **Goal**: >80% for production code
- **Current Target**: 90%+ for Swagger configuration code

### Branch Coverage

- **What it measures**: Percentage of conditional branches tested
- **Includes**: `if`, `switch`, `ternary`, `??` operators
- **Goal**: >70% for production code

## Current Coverage Status

Run `.\run-coverage.ps1` to see current metrics!

**Expected Coverage:**

- ✅ Filters: ~95%+ (highly testable)
- ✅ Configuration Classes: ~90%+ (straightforward logic)
- ✅ Extensions: ~85%+ (some edge cases)
- ⚠️ Total Project: Aim for >85%

## Excluding Files from Coverage

To exclude specific files/classes from coverage analysis, add to `.runsettings`:

```xml
<ModulePaths>
  <Exclude>
    <ModulePath>.*Tests.dll</ModulePath>
    <ModulePath>.*\.Views\.dll</ModulePath>
  </Exclude>
</ModulePaths>
```

## CI/CD Integration

### GitHub Actions Example

```yaml
- name: Test with Coverage
  run: |
    dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

- name: Generate Coverage Report
  run: |
    reportgenerator -reports:./coverage/**/coverage.cobertura.xml \
                    -targetdir:./coverage/html \
                    -reporttypes:Html;Badges

- name: Upload Coverage Artifact
  uses: actions/upload-artifact@v3
  with:
    name: coverage-report
    path: ./coverage/html
```

### Azure DevOps Example

```yaml
- task: DotNetCoreCLI@2
  displayName: "Run Tests with Coverage"
  inputs:
    command: test
    arguments: '--collect:"XPlat Code Coverage"'

- task: reportgenerator@5
  displayName: "Generate Coverage Report"
  inputs:
    reports: "$(Agent.TempDirectory)/**/coverage.cobertura.xml"
    targetdir: "$(Build.ArtifactStagingDirectory)/coverage"
```

## Troubleshooting

### "Coverage file not found"

- Ensure tests are passing
- Check that `coverlet.collector` package is installed
- Verify test project references WebApi project

### "Report generation failed"

- Ensure `dotnet-reportgenerator-globaltool` is installed:
  ```bash
  dotnet tool install -g dotnet-reportgenerator-globaltool
  ```
- Check .NET SDK version compatibility

### "Report not opening in Edge"

- Verify Edge is installed at default location
- Manually open: `.\coverage\html\index.html`
- Use `run-coverage-quick.ps1` to retry opening

### Low Coverage on Auto-Generated Code

- Exclude auto-generated files from coverage
- Focus coverage efforts on business logic
- Consider marking auto-generated classes with `[ExcludeFromCodeCoverage]`

## Coverage Best Practices

### ✅ DO

- **Test public APIs thoroughly** - All public methods should have tests
- **Cover edge cases** - Null values, empty collections, boundary conditions
- **Test error paths** - Exception handling and validation
- **Keep tests fast** - Coverage should run quickly in CI/CD

### ❌ DON'T

- **Chase 100% coverage** - Diminishing returns after ~90%
- **Test trivial code** - Auto-properties, simple getters/setters
- **Write tests just for coverage** - Tests should add value
- **Ignore code quality for coverage** - Both matter equally

## Resources

- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator Documentation](https://github.com/danielpalme/ReportGenerator)
- [Microsoft Code Coverage Guide](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage)

## Questions?

For issues or questions about test coverage, refer to:

- `src/WebApi.Tests/README.md` - Test suite documentation
- `src/WebApi/StartupExtensions/Swagger/README.md` - Architecture overview
