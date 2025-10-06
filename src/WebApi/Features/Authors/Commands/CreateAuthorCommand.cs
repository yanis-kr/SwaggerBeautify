using MediatR;
using WebApi.Attributes;
using WebApi.Features.Authors.Models;

namespace WebApi.Features.Authors.Commands;

public record CreateAuthorCommand(CreateAuthorRequest Request) : IRequest<AuthorDto>;

/// <summary>
/// Request model for creating a new author
/// </summary>
public class CreateAuthorRequest
{
    /// <summary>
    /// The full name of the author
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The email address of the author
    /// </summary>
    /// <example>john.doe@example.com</example>
    [SwaggerProps(Example = "john.doe@example.com", Format = "email")]
    public string Email { get; set; } = string.Empty;
}

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    private static readonly List<AuthorDto> _authors = new();
    private static int _nextId = 1;

    public Task<AuthorDto> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = new AuthorDto
        {
            Id = _nextId++,
            Name = request.Request.Name,
            Email = request.Request.Email,
            CreatedAt = DateTime.UtcNow
        };

        _authors.Add(author);
        return Task.FromResult(author);
    }
}
