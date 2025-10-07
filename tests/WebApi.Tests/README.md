# WebApi.Tests - Swagger Code Test Coverage

Comprehensive xUnit test suite for all Swagger/OpenAPI configuration code.

## Test Statistics

- **Total Tests**: 86
- **Test Status**: ✅ All Passing
- **Test Framework**: xUnit
- **Assertion Library**: FluentAssertions
- **Mocking Library**: NSubstitute

## Test Structure

```
WebApi.Tests/
├── Filters/                                          # Operation and Schema Filter Tests
│   ├── CorrelationIdOperationFilterTests.cs         # 6 tests
│   ├── JsonOnlyOperationFilterTests.cs              # 8 tests
│   ├── RemoveAdditionalPropertiesFilterTests.cs     # 7 tests
│   └── SwaggerPropsSchemaFilterTests.cs             # 22 tests
├── DocumentFilters/                                  # Document Filter Tests
│   └── ServersDocumentFilterTests.cs                # 7 tests
├── Extensions/                                       # Extension Method Tests
│   ├── SwaggerServiceExtensionsTests.cs             # 4 tests
│   ├── SwaggerApplicationExtensionsTests.cs         # 2 tests
│   └── SwaggerSecurityTests.cs                      # 17 tests
└── Infrastructure/                                   # Infrastructure Tests
    ├── MediatorTests.cs                             # 12 tests
    ├── UnitTests.cs                                 # 6 tests
    └── MediatorExtensionsTests.cs                   # 6 tests
```

## Coverage by Component

### 1. CorrelationIdOperationFilter (6 tests)

✅ Parameter addition  
✅ Response header configuration  
✅ Null handling for parameters  
✅ Null handling for responses  
✅ Example value generation  
✅ UUID format validation

### 2. JsonOnlyOperationFilter (8 tests)

✅ Request body filtering  
✅ Response content filtering  
✅ Multiple content type handling  
✅ Null request body handling  
✅ Null response handling  
✅ Missing JSON content type handling  
✅ Content preservation logic  
✅ Multiple response codes

### 3. RemoveAdditionalPropertiesFilter (7 tests)

✅ Object schema modification  
✅ AdditionalPropertiesAllowed flag  
✅ AdditionalProperties nullification  
✅ Non-object type preservation (string, array, null)  
✅ Case-insensitive type matching  
✅ Schema integrity

### 4. SwaggerPropsSchemaFilter (23 tests)

✅ Example value setting  
✅ Format specification  
✅ Description handling  
✅ ReadOnly property  
✅ WriteOnly property  
✅ Minimum/Maximum handling  
✅ MinLength/MaxLength handling  
✅ Pattern validation  
✅ Deprecated field marking  
✅ Property hiding (Hide = true)  
✅ Class hiding (class-level Hide)  
✅ Null/empty attribute handling  
✅ Multiple property types (int, string, bool, double)  
✅ CamelCase property naming

### 5. ServersDocumentFilter (7 tests)

✅ Server list creation  
✅ Development server configuration  
✅ QA server configuration  
✅ Server count validation  
✅ Existing server replacement  
✅ HTTPS enforcement  
✅ Server order validation

### 6. SwaggerServiceExtensions (4 tests)

✅ Service collection return  
✅ SwaggerGenOptions registration  
✅ Multiple service registration  
✅ No-throw guarantee

### 7. SwaggerApplicationExtensions (2 tests)

✅ ApiVersionDescriptionProvider requirement  
✅ SwaggerOptions validation

### 8. JWT Bearer Security Configuration (17 tests)

✅ Bearer security definition registration  
✅ Security scheme type (Http)  
✅ Scheme name ("bearer")  
✅ Bearer format ("JWT")  
✅ Parameter location (Header)  
✅ Authorization header name  
✅ Description presence and content  
✅ Security requirement registration  
✅ Bearer scheme reference  
✅ Reference type validation  
✅ Empty scopes for OAuth  
✅ Example token in description  
✅ Single Bearer definition  
✅ Single security requirement  
✅ Case-insensitive scheme matching  
✅ JWT token format in description

### 9. Custom Mediator Implementation (12 tests)

✅ Request/response handling  
✅ Handler resolution from DI  
✅ Null request validation  
✅ Unit return type support  
✅ Cancellation token passing  
✅ Multiple handler resolution  
✅ Missing handler error handling

### 10. Unit Type (6 tests)

✅ Default value behavior  
✅ Equality comparison  
✅ Hash code consistency  
✅ String representation  
✅ Object equality

### 11. Mediator Extensions (6 tests)

✅ Mediator registration  
✅ Service collection fluent API  
✅ Auto-registration of handlers  
✅ Scoped lifetime  
✅ Handler lifetime configuration

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run with Detailed Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName~SwaggerPropsSchemaFilterTests"
```

### Run Tests with Coverage (requires coverlet)

```bash
dotnet test /p:CollectCoverage=true /p:CoverageReportsFormat=opencover
```

## Test Patterns Used

### 1. **Arrange-Act-Assert (AAA)**

All tests follow the AAA pattern for clarity and maintainability.

### 2. **Descriptive Test Names**

Test names clearly describe what is being tested and expected behavior:

- `Apply_WhenPropertyHasExample_ShouldSetExample`
- `Apply_WhenSchemaTypeIsObject_ShouldSetAdditionalPropertiesAllowedToTrue`

### 3. **Theory Tests**

Used for testing multiple scenarios with different inputs:

```csharp
[Theory]
[InlineData("object")]
[InlineData("Object")]
[InlineData("OBJECT")]
public void Apply_ShouldBeCaseInsensitiveForObjectType(string typeValue)
```

### 4. **FluentAssertions**

Readable and expressive assertions:

```csharp
schema.Properties["name"].Example.Should().NotBeNull();
_operation.Parameters.Should().HaveCount(1);
```

### 5. **NSubstitute Mocking**

Clean mocking for dependencies:

```csharp
var context = CreateOperationFilterContext();
Substitute.For<ISchemaGenerator>()
```

## Dependencies

```xml
<PackageReference Include="xunit" />
<PackageReference Include="xunit.runner.visualstudio" />
<PackageReference Include="FluentAssertions" />
<PackageReference Include="NSubstitute" />
<PackageReference Include="Microsoft.AspNetCore.TestHost" />
```

## Key Testing Scenarios Covered

### ✅ Happy Path Scenarios

- Valid inputs produce expected outputs
- All filters apply correctly
- Configuration methods work as expected

### ✅ Edge Cases

- Null values
- Empty collections
- Missing properties
- Case variations

### ✅ Error Handling

- Invalid configurations throw appropriate exceptions
- Missing dependencies are detected
- Null safety is maintained

### ✅ Integration Points

- Filter interactions
- Service registration
- Middleware configuration

## Continuous Improvement

### Future Enhancements

1. Add code coverage reporting
2. Add mutation testing
3. Add performance benchmarks
4. Add integration tests with real ASP.NET Core app

### Maintenance

- Keep tests updated with code changes
- Add tests for new features
- Refactor tests as code evolves
- Monitor test execution time

## Contributing

When adding new Swagger functionality:

1. Write tests first (TDD approach)
2. Ensure all existing tests pass
3. Aim for >80% code coverage
4. Follow existing test patterns
5. Use descriptive test names

## Test Execution in CI/CD

These tests are designed to run in:

- Local development environments
- Pull request validation
- CI/CD pipelines
- Automated deployment checks

## Contact

For questions about tests or to report issues, please refer to the main project documentation.
