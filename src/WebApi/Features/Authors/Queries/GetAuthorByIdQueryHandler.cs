using MediatR;
using WebApi.Models.Authors;

namespace WebApi.Features.Authors.Queries;

public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, Author?>
{
    private static readonly List<Author> _authors = new()
    {
        new Author { Id = 1, Name = "John Doe", Email = "john.doe@example.com", CreatedAt = DateTime.UtcNow },
        new Author { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", CreatedAt = DateTime.UtcNow }
    };

    public Task<Author?> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = _authors.FirstOrDefault(a => a.Id == request.Id);
        return Task.FromResult(author);
    }
}
