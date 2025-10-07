using FluentAssertions;
using Microsoft.OpenApi.Models;
using NSubstitute;
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
        _document.Servers.Should().NotBeNull();
        _document.Servers.Should().NotBeEmpty();
    }

    [Fact]
    public void Apply_ShouldAddTwoServers()
    {
        // Act
        _sut.Apply(_document, _context);

        // Assert
        _document.Servers.Should().HaveCount(2);
    }

    [Fact]
    public void Apply_ShouldAddDevServer()
    {
        // Act
        _sut.Apply(_document, _context);

        // Assert
        var devServer = _document.Servers.FirstOrDefault(s => s.Url == "https://dev.local");
        devServer.Should().NotBeNull();
        devServer!.Description.Should().Be("Development Environment");
    }

    [Fact]
    public void Apply_ShouldAddQaServer()
    {
        // Act
        _sut.Apply(_document, _context);

        // Assert
        var qaServer = _document.Servers.FirstOrDefault(s => s.Url == "https://qa.local");
        qaServer.Should().NotBeNull();
        qaServer!.Description.Should().Be("QA Environment");
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
        _document.Servers.Should().HaveCount(2);
        _document.Servers.Should().NotContain(s => s.Url == "https://old.example.com");
    }

    [Fact]
    public void Apply_ServerUrlsShouldUseHttps()
    {
        // Act
        _sut.Apply(_document, _context);

        // Assert
        _document.Servers.Should().OnlyContain(s => s.Url.StartsWith("https://"));
    }

    [Fact]
    public void Apply_ShouldAddServersInCorrectOrder()
    {
        // Act
        _sut.Apply(_document, _context);

        // Assert
        _document.Servers[0].Url.Should().Be("https://dev.local");
        _document.Servers[1].Url.Should().Be("https://qa.local");
    }

    private static DocumentFilterContext CreateDocumentFilterContext()
    {
        var apiDescriptions = new List<Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription>();
        var schemaRepository = new SchemaRepository();
        
        return new DocumentFilterContext(
            apiDescriptions,
            Substitute.For<ISchemaGenerator>(),
            schemaRepository);
    }
}

