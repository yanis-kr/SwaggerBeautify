using WebApi.Infrastructure.Mediator;
using WebApi.Models;

namespace WebApi.Features.Authors.Commands;

public record UpdateAuthorCommand(int Id, UpdateAuthorRequest Request, CommonParameters CommonParams) : IRequest;

/// <summary>
/// Request model for updating an existing author
/// </summary>
public class UpdateAuthorRequest
{
    /// <summary>
    /// The full name of the author
    /// </summary>
    /// <example>Jane Smith</example>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The email address of the author
    /// </summary>
    /// <example>jane.smith@example.com</example>
    public string Email { get; set; } = string.Empty;
}

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand>
{
    public Task<Unit> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        // This would typically update the author in a database
        // For in-memory implementation, we'll just simulate success
        return Task.FromResult(Unit.Value);
    }
}
