using MediatR;
using WebApi.Models.Authors;

namespace WebApi.Features.Authors.Queries;

public class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, IEnumerable<Author>>
{
    private static readonly List<Author> _authors = new()
    {
        new Author { Id = 1, Name = "John Doe", Email = "john.doe@example.com", CreatedAt = DateTime.UtcNow },
        new Author { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", CreatedAt = DateTime.UtcNow }
    };

    public Task<IEnumerable<Author>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_authors.AsEnumerable());
    }
}
