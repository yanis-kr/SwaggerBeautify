using MediatR;
using WebApi.Features.Authors.Models;

namespace WebApi.Features.Authors.Commands;

public record CreateAuthorCommand(CreateAuthorRequest Request) : IRequest<AuthorDto>;

public class CreateAuthorRequest
{
    public string Name { get; set; } = string.Empty;
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
