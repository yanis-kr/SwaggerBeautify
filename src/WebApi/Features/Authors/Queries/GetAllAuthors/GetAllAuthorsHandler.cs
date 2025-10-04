using MediatR;
using WebApi.Features.Authors.Models;

namespace WebApi.Features.Authors.Queries.GetAllAuthors;

public class GetAllAuthorsHandler : IRequestHandler<GetAllAuthorsQuery, IEnumerable<AuthorDto>>
{
    private static readonly List<AuthorDto> _authors = new()
    {
        new AuthorDto { Id = 1, Name = "John Doe", Email = "john.doe@example.com", CreatedAt = DateTime.UtcNow },
        new AuthorDto { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", CreatedAt = DateTime.UtcNow }
    };

    public Task<IEnumerable<AuthorDto>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_authors.AsEnumerable());
    }
}