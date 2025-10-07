using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.StartupExtensions.Swagger.DocumentFilters;

namespace WebApi.Tests.DocumentFilters;

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
        var apiDescriptions = new List<Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription>();
        var schemaRepository = new SchemaRepository();
        
        return new DocumentFilterContext(
            apiDescriptions,
            new TestSchemaGenerator(),
            schemaRepository);
    }

    private class TestSchemaGenerator : ISchemaGenerator
    {
        public OpenApiSchema GenerateSchema(Type type, SchemaRepository schemaRepository, 
            System.Reflection.MemberInfo? memberInfo = null, System.Reflection.ParameterInfo? parameterInfo = null, 
            ApiParameterRouteInfo? routeInfo = null)
        {
            return new OpenApiSchema();
        }
    }
}

