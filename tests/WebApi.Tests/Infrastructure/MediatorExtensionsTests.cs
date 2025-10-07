using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Infrastructure.Mediator;
using WebApi.StartupExtensions;

namespace WebApi.Tests.Infrastructure;

public class MediatorExtensionsTests
{
    [Fact]
    public void AddMediatorSetup_ShouldRegisterMediator()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediatorSetup();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetService<IMediator>();
        mediator.Should().NotBeNull();
        mediator.Should().BeOfType<Mediator>();
    }

    [Fact]
    public void AddMediatorSetup_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddMediatorSetup();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddMediatorSetup_ShouldAutoRegisterHandlersFromAssembly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediatorSetup();

        // Assert
        // Check that handlers are registered (they're in the WebApi assembly)
        services.Should().Contain(sd => sd.ServiceType.Name.StartsWith("IRequestHandler"));
    }

    [Fact]
    public void AddMediatorSetup_ShouldRegisterMediatorAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediatorSetup();

        // Assert
        var descriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(IMediator));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddMediatorSetup_ShouldRegisterHandlersAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediatorSetup();

        // Assert
        var handlerDescriptors = services.Where(sd => sd.ServiceType.Name.StartsWith("IRequestHandler"));
        handlerDescriptors.Should().AllSatisfy(d => d.Lifetime.Should().Be(ServiceLifetime.Scoped));
    }

    [Fact]
    public void AddMediatorSetup_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddMediatorSetup();

        // Assert
        act.Should().NotThrow();
    }
}

