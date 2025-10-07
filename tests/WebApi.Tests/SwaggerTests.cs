using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using WebApi.Attributes;
using WebApi.StartupExtensions.Swagger;
using WebApi.StartupExtensions.Swagger.DocumentFilters;
using WebApi.StartupExtensions.Swagger.Filters;

namespace WebApi.Tests;

public class SwaggerTests
{
    private class TestSchemaGenerator : ISchemaGenerator
    {
        public OpenApiSchema GenerateSchema(Type type, SchemaRepository schemaRepository, 
            System.Reflection.MemberInfo? memberInfo = null, System.Reflection.ParameterInfo? parameterInfo = null, 
            ApiParameterRouteInfo? routeInfo = null)
        {
            return new OpenApiSchema();
        }
    }

    #region ServersDocumentFilter Tests

    public class ServersDocumentFilterTests
    {
        private readonly ServersDocumentFilter _sut;
        private readonly OpenApiDocument _document;
        private readonly DocumentFilterContext _context;

        public ServersDocumentFilterTests()
        {
            _sut = new ServersDocumentFilter();
            _document = new OpenApiDocument
            {
                Info = new OpenApiInfo { Title = "Test API", Version = "v1" },
                Paths = new OpenApiPaths()
            };
            _context = CreateDocumentFilterContext();
        }

        [Fact]
        public void Apply_ShouldAddServersList()
        {
            // Act
            _sut.Apply(_document, _context);

            // Assert
            Assert.NotNull(_document.Servers);
            Assert.NotEmpty(_document.Servers);
        }

        [Fact]
        public void Apply_ShouldAddLocalhostServer()
        {
            // Act
            _sut.Apply(_document, _context);

            // Assert
            var localhostServer = _document.Servers.FirstOrDefault(s => s.Url == "https://localhost:60983");
            Assert.NotNull(localhostServer);
            Assert.Equal("Local Development", localhostServer.Description);
        }

        [Fact]
        public void Apply_ShouldAddFourServers()
        {
            // Act
            _sut.Apply(_document, _context);

            // Assert
            Assert.Equal(4, _document.Servers.Count);
        }

        [Fact]
        public void Apply_ShouldAddDevServer()
        {
            // Act
            _sut.Apply(_document, _context);

            // Assert
            var devServer = _document.Servers.FirstOrDefault(s => s.Url == "https://dev.local");
            Assert.NotNull(devServer);
            Assert.Equal("Development Environment", devServer.Description);
        }

        [Fact]
        public void Apply_ShouldAddQaServer()
        {
            // Act
            _sut.Apply(_document, _context);

            // Assert
            var qaServer = _document.Servers.FirstOrDefault(s => s.Url == "https://qa.local");
            Assert.NotNull(qaServer);
            Assert.Equal("QA Environment", qaServer.Description);
        }

        [Fact]
        public void Apply_ShouldAddUatServer()
        {
            // Act
            _sut.Apply(_document, _context);

            // Assert
            var uatServer = _document.Servers.FirstOrDefault(s => s.Url == "https://uat.local");
            Assert.NotNull(uatServer);
            Assert.Equal("UAT Environment", uatServer.Description);
        }

        [Fact]
        public void Apply_ShouldReplaceExistingServers()
        {
            // Arrange
            _document.Servers = new List<OpenApiServer>
            {
                new OpenApiServer { Url = "https://old.example.com", Description = "Old Server" }
            };

            // Act
            _sut.Apply(_document, _context);

            // Assert
            Assert.Equal(4, _document.Servers.Count);
            Assert.DoesNotContain(_document.Servers, s => s.Url == "https://old.example.com");
        }

        [Fact]
        public void Apply_ServerUrlsShouldUseHttps()
        {
            // Act
            _sut.Apply(_document, _context);

            // Assert
            Assert.All(_document.Servers, s => Assert.StartsWith("https://", s.Url));
        }

        [Fact]
        public void Apply_ShouldAddServersInCorrectOrder()
        {
            // Act
            _sut.Apply(_document, _context);

            // Assert
            Assert.Equal("https://localhost:60983", _document.Servers[0].Url);
            Assert.Equal("Local Development", _document.Servers[0].Description);
            Assert.Equal("https://dev.local", _document.Servers[1].Url);
            Assert.Equal("Development Environment", _document.Servers[1].Description);
            Assert.Equal("https://qa.local", _document.Servers[2].Url);
            Assert.Equal("QA Environment", _document.Servers[2].Description);
            Assert.Equal("https://uat.local", _document.Servers[3].Url);
            Assert.Equal("UAT Environment", _document.Servers[3].Description);
        }

        private static DocumentFilterContext CreateDocumentFilterContext()
        {
            var apiDescriptions = new List<ApiDescription>();
            var schemaRepository = new SchemaRepository();
            
            return new DocumentFilterContext(
                apiDescriptions,
                new TestSchemaGenerator(),
                schemaRepository);
        }
    }

    #endregion

    #region CommonResponseHeadersFilter Tests

    public class CommonResponseHeadersFilterTests
    {
        private readonly CommonResponseHeadersFilter _sut;
        private readonly OpenApiOperation _operation;
        private readonly OperationFilterContext _context;

        public CommonResponseHeadersFilterTests()
        {
            _sut = new CommonResponseHeadersFilter();
            _operation = new OpenApiOperation();
            _context = CreateOperationFilterContext();
        }

        [Fact]
        public void Apply_ShouldAddCorrelationIdToAllResponses()
        {
            // Arrange
            _operation.Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse { Description = "Success" },
                ["400"] = new OpenApiResponse { Description = "Bad Request" },
                ["404"] = new OpenApiResponse { Description = "Not Found" }
            };

            // Act
            _sut.Apply(_operation, _context);

            // Assert
            foreach (var response in _operation.Responses.Values)
            {
                Assert.True(response.Headers.ContainsKey("Correlation-Id"));
                var header = response.Headers["Correlation-Id"];
                Assert.Contains("correlation ID", header.Description);
                Assert.Equal("string", header.Schema.Type);
                Assert.Equal("uuid", header.Schema.Format);
                Assert.NotNull(header.Schema.Example);
            }
        }

        [Fact]
        public void Apply_WhenResponsesIsNull_ShouldInitializeResponses()
        {
            // Arrange
            _operation.Responses = null;

            // Act
            _sut.Apply(_operation, _context);

            // Assert
            Assert.NotNull(_operation.Responses);
        }

        [Fact]
        public void Apply_WhenResponseHeadersIsNull_ShouldInitializeHeaders()
        {
            // Arrange
            _operation.Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse 
                { 
                    Description = "Success",
                    Headers = null
                }
            };

            // Act
            _sut.Apply(_operation, _context);

            // Assert
            Assert.NotNull(_operation.Responses["200"].Headers);
            Assert.True(_operation.Responses["200"].Headers.ContainsKey("Correlation-Id"));
        }

        [Fact]
        public void Apply_WhenCorrelationIdAlreadyExists_ShouldNotOverwrite()
        {
            // Arrange
            var existingHeader = new OpenApiHeader
            {
                Description = "Custom description",
                Schema = new OpenApiSchema { Type = "string" }
            };

            _operation.Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse 
                { 
                    Description = "Success",
                    Headers = new Dictionary<string, OpenApiHeader>
                    {
                        ["Correlation-Id"] = existingHeader
                    }
                }
            };

            // Act
            _sut.Apply(_operation, _context);

            // Assert
            Assert.Same(existingHeader, _operation.Responses["200"].Headers["Correlation-Id"]);
            Assert.Equal("Custom description", _operation.Responses["200"].Headers["Correlation-Id"].Description);
        }

        [Fact]
        public void Apply_ShouldSetCorrectSchemaFormat()
        {
            // Arrange
            _operation.Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse { Description = "Success" }
            };

            // Act
            _sut.Apply(_operation, _context);

            // Assert
            var header = _operation.Responses["200"].Headers["Correlation-Id"];
            Assert.Equal("uuid", header.Schema.Format);
            Assert.Equal("string", header.Schema.Type);
        }

        private static OperationFilterContext CreateOperationFilterContext()
        {
            var apiDescription = new ApiDescription();
            var schemaRepository = new SchemaRepository();
            var methodInfo = typeof(CommonResponseHeadersFilterTests).GetMethod(nameof(CreateOperationFilterContext));
            
            return new OperationFilterContext(
                apiDescription,
                new TestSchemaGenerator(),
                schemaRepository,
                methodInfo!);
        }
    }

    #endregion

    #region SwaggerPropsParameterFilter Tests

    public class SwaggerPropsParameterFilterTests
    {
        private readonly SwaggerPropsParameterFilter _sut;

        public SwaggerPropsParameterFilterTests()
        {
            _sut = new SwaggerPropsParameterFilter();
        }

        [Fact]
        public void Apply_ShouldNotThrow_WhenParameterInfoIsNull()
        {
            // Arrange
            var parameter = new OpenApiParameter
            {
                Name = "test-param",
                In = ParameterLocation.Query
            };

            var context = CreateParameterFilterContext(null);

            // Act & Assert
            var exception = Record.Exception(() => _sut.Apply(parameter, context));
            Assert.Null(exception);
        }

        [Fact]
        public void Apply_ShouldApplySwaggerPropsAttributes_ToParameterWithAttribute()
        {
            // Arrange
            var parameter = new OpenApiParameter
            {
                Name = "Correlation-Id",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema()
            };

            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod));
            var parameterInfo = methodInfo!.GetParameters().First(p => p.Name == "correlationId");
            
            var context = CreateParameterFilterContext(parameterInfo);

            // Act
            _sut.Apply(parameter, context);

            // Assert
            Assert.NotNull(parameter.Schema);
            Assert.Equal("uuid", parameter.Schema.Format);
            Assert.NotNull(parameter.Schema.Example);
            Assert.IsType<OpenApiString>(parameter.Schema.Example);
            var exampleString = parameter.Schema.Example as OpenApiString;
            Assert.Equal("12345678-1234-1234-1234-123456789abc", exampleString!.Value);
        }

        [Fact]
        public void Apply_ShouldApplyDescription_WhenSwaggerPropsHasDescription()
        {
            // Arrange
            var parameter = new OpenApiParameter
            {
                Name = "test-param",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema()
            };

            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod));
            var parameterInfo = methodInfo!.GetParameters().First(p => p.Name == "userContext");
            
            var context = CreateParameterFilterContext(parameterInfo);

            // Act
            _sut.Apply(parameter, context);

            // Assert
            Assert.Equal("Test description for user context", parameter.Description);
        }

        [Fact]
        public void Apply_ShouldInitializeSchema_WhenSchemaIsNull()
        {
            // Arrange
            var parameter = new OpenApiParameter
            {
                Name = "test-param",
                In = ParameterLocation.Header,
                Schema = null
            };

            var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod));
            var parameterInfo = methodInfo!.GetParameters().First(p => p.Name == "correlationId");
            
            var context = CreateParameterFilterContext(parameterInfo);

            // Act
            _sut.Apply(parameter, context);

            // Assert
            Assert.NotNull(parameter.Schema);
        }

        private static ParameterFilterContext CreateParameterFilterContext(ParameterInfo? parameterInfo)
        {
            var schemaRepository = new SchemaRepository();
            var apiParameterDescription = new ApiParameterDescription();

            return new ParameterFilterContext(
                apiParameterDescription,
                new TestSchemaGenerator(),
                schemaRepository,
                null,
                parameterInfo);
        }

        private class TestClass
        {
            public void TestMethod(
                [SwaggerProps(Format = "uuid", Example = "12345678-1234-1234-1234-123456789abc")]
                Guid correlationId,
                [SwaggerProps(Description = "Test description for user context")]
                string userContext)
            {
            }
        }
    }

    #endregion

    #region SwaggerPropsSchemaFilter Tests

    public class SwaggerPropsSchemaFilterTests
    {
        private readonly SwaggerPropsSchemaFilter _sut;

        public SwaggerPropsSchemaFilterTests()
        {
            _sut = new SwaggerPropsSchemaFilter();
        }

        [Fact]
        public void Apply_WhenPropertyHasExample_ShouldSetExample()
        {
            // Arrange
            var schema = CreateSchema();
            var context = CreateSchemaFilterContext(typeof(DtoWithExample));

            // Act
            _sut.Apply(schema, context);

            // Assert
            Assert.NotNull(schema.Properties["name"].Example);
        }

        [Fact]
        public void Apply_WhenPropertyHasFormat_ShouldSetFormat()
        {
            // Arrange
            var schema = CreateSchema();
            var context = CreateSchemaFilterContext(typeof(DtoWithFormat));

            // Act
            _sut.Apply(schema, context);

            // Assert
            Assert.Equal("email", schema.Properties["email"].Format);
        }

        [Fact]
        public void Apply_WhenPropertyHasDescription_ShouldSetDescription()
        {
            // Arrange
            var schema = CreateSchema();
            var context = CreateSchemaFilterContext(typeof(DtoWithDescription));

            // Act
            _sut.Apply(schema, context);

            // Assert
            Assert.Equal("User's full name", schema.Properties["name"].Description);
        }

        [Fact]
        public void Apply_WhenPropertyIsReadOnly_ShouldSetReadOnly()
        {
            // Arrange
            var schema = CreateSchema();
            var context = CreateSchemaFilterContext(typeof(DtoWithReadOnly));

            // Act
            _sut.Apply(schema, context);

            // Assert
            Assert.True(schema.Properties["id"].ReadOnly);
        }

        [Fact]
        public void Apply_WhenPropertyIsWriteOnly_ShouldSetWriteOnly()
        {
            // Arrange
            var schema = CreateSchema();
            var context = CreateSchemaFilterContext(typeof(DtoWithWriteOnly));

            // Act
            _sut.Apply(schema, context);

            // Assert
            Assert.True(schema.Properties["password"].WriteOnly);
        }

        [Fact]
        public void Apply_WhenPropertyHasPattern_ShouldSetPattern()
        {
            // Arrange
            var schema = CreateSchema();
            var context = CreateSchemaFilterContext(typeof(DtoWithPattern));

            // Act
            _sut.Apply(schema, context);

            // Assert
            Assert.Equal("^[A-Z]{3}$", schema.Properties["code"].Pattern);
        }

        [Fact]
        public void Apply_WhenPropertyIsDeprecated_ShouldSetDeprecatedAndAddToDescription()
        {
            // Arrange
            var schema = CreateSchema();
            var context = CreateSchemaFilterContext(typeof(DtoWithDeprecated));

            // Act
            _sut.Apply(schema, context);

            // Assert
            Assert.True(schema.Properties["oldField"].Deprecated);
            Assert.Contains("DEPRECATED", schema.Properties["oldField"].Description);
            Assert.Contains("Use newField instead", schema.Properties["oldField"].Description);
        }

        [Fact]
        public void Apply_WhenPropertyHasHideTrue_ShouldRemoveProperty()
        {
            // Arrange
            var schema = CreateSchemaWithProperty("hiddenField");
            var context = CreateSchemaFilterContext(typeof(DtoWithHidden));

            // Act
            _sut.Apply(schema, context);

            // Assert
            Assert.False(schema.Properties.ContainsKey("hiddenField"));
        }

        [Fact]
        public void Apply_WhenClassHasHideTrue_ShouldClearPropertiesAndType()
        {
            // Arrange
            var schema = CreateSchema();
            var context = CreateSchemaFilterContext(typeof(HiddenDto));

            // Act
            _sut.Apply(schema, context);

            // Assert
            Assert.Empty(schema.Properties);
            Assert.Null(schema.Type);
            Assert.Equal("Hidden from API documentation", schema.Description);
        }

        [Fact]
        public void Apply_WhenNoAttributes_ShouldNotModifySchema()
        {
            // Arrange
            var schema = CreateSchema();
            var originalProperties = schema.Properties.Count;
            var context = CreateSchemaFilterContext(typeof(PlainDto));

            // Act
            _sut.Apply(schema, context);

            // Assert
            Assert.Equal(originalProperties, schema.Properties.Count);
        }

        [Theory]
        [InlineData(123)]
        [InlineData("test")]
        [InlineData(true)]
        [InlineData(45.67)]
        public void Apply_ShouldHandleDifferentExampleTypes(object exampleValue)
        {
            // Arrange
            var schema = CreateSchema();
            var context = CreateSchemaFilterContext(typeof(DtoWithExample));

            // Act & Assert
            var exception = Record.Exception(() => _sut.Apply(schema, context));
            Assert.Null(exception);
            
            var exampleType = exampleValue.GetType();
            Assert.True(
                exampleType == typeof(int) || exampleType == typeof(string) || 
                exampleType == typeof(bool) || exampleType == typeof(double),
                "the example value should be one of the supported types");
        }

        private static OpenApiSchema CreateSchema()
        {
            return new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["name"] = new OpenApiSchema { Type = "string" },
                    ["email"] = new OpenApiSchema { Type = "string" },
                    ["id"] = new OpenApiSchema { Type = "integer" },
                    ["password"] = new OpenApiSchema { Type = "string" },
                    ["age"] = new OpenApiSchema { Type = "integer" },
                    ["code"] = new OpenApiSchema { Type = "string" },
                    ["oldField"] = new OpenApiSchema { Type = "string" }
                }
            };
        }

        private static OpenApiSchema CreateSchemaWithProperty(string propertyName)
        {
            return new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    [propertyName] = new OpenApiSchema { Type = "string" }
                }
            };
        }

        private static SchemaFilterContext CreateSchemaFilterContext(Type type)
        {
            var schemaRepository = new SchemaRepository();
            return new SchemaFilterContext(
                type,
                new TestSchemaGenerator(),
                schemaRepository);
        }

        private class DtoWithExample
        {
            [SwaggerProps(Example = "John Doe")]
            public string Name { get; set; } = string.Empty;
        }

        private class DtoWithFormat
        {
            [SwaggerProps(Format = "email")]
            public string Email { get; set; } = string.Empty;
        }

        private class DtoWithDescription
        {
            [SwaggerProps(Description = "User's full name")]
            public string Name { get; set; } = string.Empty;
        }

        private class DtoWithReadOnly
        {
            [SwaggerProps(ReadOnly = true)]
            public int Id { get; set; }
        }

        private class DtoWithWriteOnly
        {
            [SwaggerProps(WriteOnly = true)]
            public string Password { get; set; } = string.Empty;
        }

        private class DtoWithPattern
        {
            [SwaggerProps(Pattern = "^[A-Z]{3}$")]
            public string Code { get; set; } = string.Empty;
        }

        private class DtoWithDeprecated
        {
            [SwaggerProps(Deprecated = "Use newField instead")]
            public string OldField { get; set; } = string.Empty;
        }

        private class DtoWithHidden
        {
            [SwaggerProps(Hide = true)]
            public string HiddenField { get; set; } = string.Empty;
        }

        [SwaggerProps(Hide = true)]
        private class HiddenDto
        {
            public string Name { get; set; } = string.Empty;
        }

        private class PlainDto
        {
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
        }
    }

    #endregion

    #region JsonOnlyOperationFilter Tests

    public class JsonOnlyOperationFilterTests
    {
        private readonly JsonOnlyOperationFilter _sut;
        private readonly OpenApiOperation _operation;
        private readonly OperationFilterContext _context;

        public JsonOnlyOperationFilterTests()
        {
            _sut = new JsonOnlyOperationFilter();
            _operation = new OpenApiOperation();
            _context = CreateOperationFilterContext();
        }

        [Fact]
        public void Apply_WhenRequestBodyHasMultipleContentTypes_ShouldKeepOnlyJson()
        {
            // Arrange
            var jsonMediaType = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object" } };
            var xmlMediaType = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object" } };
            
            _operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = jsonMediaType,
                    ["application/xml"] = xmlMediaType,
                    ["text/plain"] = new OpenApiMediaType()
                }
            };

            // Act
            _sut.Apply(_operation, _context);

            // Assert
            Assert.Single(_operation.RequestBody.Content);
            Assert.True(_operation.RequestBody.Content.ContainsKey("application/json"));
            Assert.Same(jsonMediaType, _operation.RequestBody.Content["application/json"]);
        }

        [Fact]
        public void Apply_WhenResponsesHaveMultipleContentTypes_ShouldKeepOnlyJson()
        {
            // Arrange
            var jsonMediaType = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object" } };
            var xmlMediaType = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "object" } };
            
            _operation.Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = jsonMediaType,
                        ["application/xml"] = xmlMediaType
                    }
                },
                ["400"] = new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType(),
                        ["text/plain"] = new OpenApiMediaType()
                    }
                }
            };

            // Act
            _sut.Apply(_operation, _context);

            // Assert
            Assert.Single(_operation.Responses["200"].Content);
            Assert.True(_operation.Responses["200"].Content.ContainsKey("application/json"));
            Assert.Same(jsonMediaType, _operation.Responses["200"].Content["application/json"]);
            
            Assert.Single(_operation.Responses["400"].Content);
            Assert.True(_operation.Responses["400"].Content.ContainsKey("application/json"));
        }

        [Fact]
        public void Apply_WhenRequestBodyIsNull_ShouldNotThrow()
        {
            // Arrange
            _operation.RequestBody = null;

            // Act & Assert
            var exception = Record.Exception(() => _sut.Apply(_operation, _context));
            Assert.Null(exception);
        }

        [Fact]
        public void Apply_WhenRequestBodyContentIsNull_ShouldNotThrow()
        {
            // Arrange
            _operation.RequestBody = new OpenApiRequestBody { Content = null };

            // Act & Assert
            var exception = Record.Exception(() => _sut.Apply(_operation, _context));
            Assert.Null(exception);
        }

        [Fact]
        public void Apply_WhenResponsesIsNull_ShouldNotThrow()
        {
            // Arrange
            _operation.Responses = null;

            // Act & Assert
            var exception = Record.Exception(() => _sut.Apply(_operation, _context));
            Assert.Null(exception);
        }

        [Fact]
        public void Apply_WhenResponseContentIsNull_ShouldNotThrow()
        {
            // Arrange
            _operation.Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse { Content = null }
            };

            // Act & Assert
            var exception = Record.Exception(() => _sut.Apply(_operation, _context));
            Assert.Null(exception);
        }

        [Fact]
        public void Apply_WhenNoJsonContentType_ShouldClearContent()
        {
            // Arrange
            _operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/xml"] = new OpenApiMediaType(),
                    ["text/plain"] = new OpenApiMediaType()
                }
            };

            // Act
            _sut.Apply(_operation, _context);

            // Assert
            Assert.False(_operation.RequestBody.Content.ContainsKey("application/json"));
        }

        private static OperationFilterContext CreateOperationFilterContext()
        {
            var apiDescription = new ApiDescription();
            var schemaRepository = new SchemaRepository();
            var methodInfo = typeof(JsonOnlyOperationFilterTests).GetMethod(nameof(CreateOperationFilterContext));
            
            return new OperationFilterContext(
                apiDescription,
                new TestSchemaGenerator(),
                schemaRepository,
                methodInfo!);
        }
    }

    #endregion

    #region RemoveAdditionalPropertiesFilter Tests

    public class RemoveAdditionalPropertiesFilterTests
    {
        private readonly RemoveAdditionalPropertiesFilter _sut;
        private readonly SchemaFilterContext _context;

        public RemoveAdditionalPropertiesFilterTests()
        {
            _sut = new RemoveAdditionalPropertiesFilter();
            _context = CreateSchemaFilterContext(typeof(TestDto));
        }

        [Fact]
        public void Apply_WhenSchemaTypeIsObject_ShouldSetAdditionalPropertiesAllowedToTrue()
        {
            // Arrange
            var schema = new OpenApiSchema { Type = "object" };

            // Act
            _sut.Apply(schema, _context);

            // Assert
            Assert.True(schema.AdditionalPropertiesAllowed);
        }

        [Fact]
        public void Apply_WhenSchemaTypeIsObject_ShouldSetAdditionalPropertiesToNull()
        {
            // Arrange
            var schema = new OpenApiSchema 
            { 
                Type = "object",
                AdditionalProperties = new OpenApiSchema { Type = "string" }
            };

            // Act
            _sut.Apply(schema, _context);

            // Assert
            Assert.Null(schema.AdditionalProperties);
        }

        [Fact]
        public void Apply_WhenSchemaTypeIsString_ShouldNotModifySchema()
        {
            // Arrange
            var schema = new OpenApiSchema 
            { 
                Type = "string",
                AdditionalProperties = new OpenApiSchema { Type = "string" }
            };

            // Act
            _sut.Apply(schema, _context);

            // Assert
            Assert.Equal("string", schema.Type);
        }

        [Fact]
        public void Apply_WhenSchemaTypeIsArray_ShouldNotModifySchema()
        {
            // Arrange
            var schema = new OpenApiSchema 
            { 
                Type = "array",
                AdditionalProperties = new OpenApiSchema { Type = "string" }
            };

            // Act
            _sut.Apply(schema, _context);

            // Assert
            Assert.Equal("array", schema.Type);
        }

        [Fact]
        public void Apply_WhenSchemaTypeIsNull_ShouldNotModifySchema()
        {
            // Arrange
            var schema = new OpenApiSchema 
            { 
                Type = null,
                AdditionalProperties = new OpenApiSchema { Type = "string" }
            };

            // Act
            _sut.Apply(schema, _context);

            // Assert
            Assert.Null(schema.Type);
        }

        [Theory]
        [InlineData("object")]
        [InlineData("Object")]
        [InlineData("OBJECT")]
        public void Apply_ShouldBeCaseInsensitiveForObjectType(string typeValue)
        {
            // Arrange
            var schema = new OpenApiSchema { Type = typeValue };

            // Act
            _sut.Apply(schema, _context);

            // Assert
            Assert.True(schema.AdditionalPropertiesAllowed);
            Assert.Null(schema.AdditionalProperties);
        }

        private static SchemaFilterContext CreateSchemaFilterContext(Type type)
        {
            var schemaRepository = new SchemaRepository();
            return new SchemaFilterContext(
                type,
                new TestSchemaGenerator(),
                schemaRepository);
        }

        private class TestDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }

    #endregion

    #region SwaggerServiceExtensions Tests

    public class SwaggerServiceExtensionsTests
    {
        [Fact]
        public void AddSwaggerDocumentation_ShouldReturnServiceCollection()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddSwaggerDocumentation();

            // Assert
            Assert.Same(services, result);
        }

        [Fact]
        public void AddSwaggerDocumentation_ShouldRegisterSwaggerGenOptions()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddSwaggerDocumentation();

            // Assert
            Assert.Contains(services, sd => sd.ServiceType == typeof(Microsoft.Extensions.Options.IConfigureOptions<SwaggerGenOptions>));
        }

        [Fact]
        public void AddSwaggerDocumentation_ShouldAddMultipleServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddSwaggerDocumentation();

            // Assert
            Assert.NotEmpty(services);
            Assert.True(services.Count > 1);
        }

        [Fact]
        public void AddSwaggerDocumentation_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            var exception = Record.Exception(() => services.AddSwaggerDocumentation());
            Assert.Null(exception);
        }
    }

    #endregion

    #region SwaggerSecurity Tests

    public class SwaggerSecurityTests
    {
        private readonly SwaggerGenOptions _options;

        public SwaggerSecurityTests()
        {
            var services = new ServiceCollection();
            services.AddSwaggerDocumentation();
            var serviceProvider = services.BuildServiceProvider();
            
            var optionsSnapshot = serviceProvider.GetService<Microsoft.Extensions.Options.IConfigureOptions<SwaggerGenOptions>>();
            _options = new SwaggerGenOptions();
            optionsSnapshot?.Configure(_options);
        }

        [Fact]
        public void AddSwaggerDocumentation_ShouldAddBearerSecurityDefinition()
        {
            // Assert
            Assert.True(_options.SwaggerGeneratorOptions.SecuritySchemes.ContainsKey("Bearer"));
        }

        [Fact]
        public void BearerSecurityDefinition_ShouldHaveCorrectType()
        {
            // Act
            var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

            // Assert
            Assert.Equal(SecuritySchemeType.Http, bearerScheme.Type);
        }

        [Fact]
        public void BearerSecurityDefinition_ShouldHaveCorrectScheme()
        {
            // Act
            var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

            // Assert
            Assert.Equal("bearer", bearerScheme.Scheme);
        }

        [Fact]
        public void BearerSecurityDefinition_ShouldHaveJWTFormat()
        {
            // Act
            var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

            // Assert
            Assert.Equal("JWT", bearerScheme.BearerFormat);
        }

        [Fact]
        public void BearerSecurityDefinition_ShouldBeInHeader()
        {
            // Act
            var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

            // Assert
            Assert.Equal(ParameterLocation.Header, bearerScheme.In);
        }

        [Fact]
        public void BearerSecurityDefinition_ShouldHaveAuthorizationName()
        {
            // Act
            var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

            // Assert
            Assert.Equal("Authorization", bearerScheme.Name);
        }

        [Fact]
        public void BearerSecurityDefinition_ShouldHaveDescription()
        {
            // Act
            var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

            // Assert
            Assert.NotNull(bearerScheme.Description);
            Assert.NotEmpty(bearerScheme.Description);
            Assert.Contains("JWT", bearerScheme.Description);
            Assert.Contains("Bearer", bearerScheme.Description);
        }

        [Fact]
        public void AddSwaggerDocumentation_ShouldAddSecurityRequirement()
        {
            // Assert
            Assert.NotEmpty(_options.SwaggerGeneratorOptions.SecurityRequirements);
        }

        [Fact]
        public void SecurityRequirement_ShouldReferenceBearerScheme()
        {
            // Act
            var securityRequirement = _options.SwaggerGeneratorOptions.SecurityRequirements.First();

            // Assert
            var bearerScheme = securityRequirement.Keys.FirstOrDefault(k => k.Reference?.Id == "Bearer");
            Assert.NotNull(bearerScheme);
        }

        [Fact]
        public void SecurityRequirement_ShouldHaveCorrectReferenceType()
        {
            // Act
            var securityRequirement = _options.SwaggerGeneratorOptions.SecurityRequirements.First();
            var bearerScheme = securityRequirement.Keys.First(k => k.Reference?.Id == "Bearer");

            // Assert
            Assert.Equal(ReferenceType.SecurityScheme, bearerScheme.Reference.Type);
        }

        [Fact]
        public void SecurityRequirement_ShouldHaveEmptyScopes()
        {
            // Act
            var securityRequirement = _options.SwaggerGeneratorOptions.SecurityRequirements.First();
            var bearerScheme = securityRequirement.Keys.First(k => k.Reference?.Id == "Bearer");
            var scopes = securityRequirement[bearerScheme];

            // Assert
            Assert.Empty(scopes);
        }

        [Fact]
        public void BearerSecurityDefinition_ShouldHaveExampleInDescription()
        {
            // Act
            var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

            // Assert
            Assert.Contains("Example:", bearerScheme.Description);
        }

        [Fact]
        public void AddSwaggerDocumentation_ShouldHaveOnlyOneBearerDefinition()
        {
            // Act
            var bearerDefinitions = _options.SwaggerGeneratorOptions.SecuritySchemes
                .Where(kvp => kvp.Key == "Bearer" || kvp.Value.Scheme == "bearer");

            // Assert
            Assert.Single(bearerDefinitions);
        }

        [Fact]
        public void AddSwaggerDocumentation_ShouldHaveOnlyOneSecurityRequirement()
        {
            // Assert
            Assert.Single(_options.SwaggerGeneratorOptions.SecurityRequirements);
        }

        [Theory]
        [InlineData("Bearer")]
        [InlineData("bearer")]
        public void BearerSecurityDefinition_ShouldMatchScheme(string expectedScheme)
        {
            // Act
            var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

            // Assert
            Assert.Equal(expectedScheme.ToLower(), bearerScheme.Scheme.ToLower());
        }

        [Fact]
        public void BearerSecurityDefinition_DescriptionShouldIncludeTokenFormat()
        {
            // Act
            var bearerScheme = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

            // Assert
            Assert.Contains("eyJ", bearerScheme.Description); // JWT token start pattern
        }
    }

    #endregion
}

