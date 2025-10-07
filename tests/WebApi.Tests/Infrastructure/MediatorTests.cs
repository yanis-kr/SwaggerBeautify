using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Infrastructure.Mediator;

namespace WebApi.Tests.Infrastructure;

public class MediatorTests
{
    [Fact]
    public async Task Send_WithValidHandler_ShouldReturnResponse()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IRequestHandler<TestQuery, string>, TestQueryHandler>();
        services.AddScoped<IMediator, Mediator>();
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var query = new TestQuery("test");

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.Should().Be("Response: test");
    }

    [Fact]
    public async Task Send_WithNoHandler_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IMediator, Mediator>();
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var query = new TestQuery("test");

        // Act
        var act = async () => await mediator.Send(query);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No handler registered*");
    }

    [Fact]
    public async Task Send_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IMediator, Mediator>();
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var act = async () => await mediator.Send<string>(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Send_WithUnitResponse_ShouldReturnUnit()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IRequestHandler<TestCommand, Unit>, TestCommandHandler>();
        services.AddScoped<IMediator, Mediator>();
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var command = new TestCommand();

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Send_ShouldPassCancellationToken()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IRequestHandler<TestCancellableQuery, bool>, TestCancellableQueryHandler>();
        services.AddScoped<IMediator, Mediator>();
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var query = new TestCancellableQuery();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await mediator.Send(query, cts.Token);

        // Assert
        result.Should().BeTrue(); // Handler checks if token was passed
    }

    [Fact]
    public async Task Send_WithMultipleHandlers_ShouldResolveCorrectOne()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IRequestHandler<TestQuery, string>, TestQueryHandler>();
        services.AddScoped<IRequestHandler<AnotherTestQuery, int>, AnotherTestQueryHandler>();
        services.AddScoped<IMediator, Mediator>();
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var stringResult = await mediator.Send(new TestQuery("value"));
        var intResult = await mediator.Send(new AnotherTestQuery());

        // Assert
        stringResult.Should().Be("Response: value");
        intResult.Should().Be(42);
    }

    // Test request and handlers
    private record TestQuery(string Value) : IRequest<string>;

    private class TestQueryHandler : IRequestHandler<TestQuery, string>
    {
        public Task<string> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult($"Response: {request.Value}");
        }
    }

    private record TestCommand : IRequest<Unit>;

    private class TestCommandHandler : IRequestHandler<TestCommand, Unit>
    {
        public Task<Unit> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Unit.Value);
        }
    }

    private record TestCancellableQuery : IRequest<bool>;

    private class TestCancellableQueryHandler : IRequestHandler<TestCancellableQuery, bool>
    {
        public Task<bool> Handle(TestCancellableQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(cancellationToken.IsCancellationRequested);
        }
    }

    private record AnotherTestQuery : IRequest<int>;

    private class AnotherTestQueryHandler : IRequestHandler<AnotherTestQuery, int>
    {
        public Task<int> Handle(AnotherTestQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(42);
        }
    }
}

