using MediatR;
using WebApi.Models.Authors;

namespace WebApi.Features.Authors.Commands;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, Author>
{
    private static readonly List<Author> _authors = new();
    private static int _nextId = 1;

    public Task<Author> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = new Author
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


