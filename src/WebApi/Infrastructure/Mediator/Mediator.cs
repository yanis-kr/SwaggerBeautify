namespace WebApi.Infrastructure.Mediator;

/// <summary>
/// Simple mediator implementation for request/response pattern
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        // Build the handler type: IRequestHandler<TRequest, TResponse>
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        // Get the handler from DI container
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new InvalidOperationException(
                $"No handler registered for request type {requestType.Name} with response type {responseType.Name}. " +
                $"Ensure the handler implements IRequestHandler<{requestType.Name}, {responseType.Name}> and is registered in DI.");
        }

        // Invoke the Handle method
        var handleMethod = handlerType.GetMethod("Handle");
        if (handleMethod == null)
        {
            throw new InvalidOperationException($"Handle method not found on handler {handlerType.Name}");
        }

        var result = handleMethod.Invoke(handler, new object[] { request, cancellationToken });

        if (result is Task<TResponse> taskResult)
        {
            return await taskResult;
        }

        throw new InvalidOperationException($"Handler did not return expected Task<{responseType.Name}>");
    }
}

