using MediatR;
using WebApi.Features.Authors.Models;

namespace WebApi.Features.Authors.Queries;

public record GetAuthorByIdQuery(int Id) : IRequest<AuthorDto?>;

public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, AuthorDto?>
{
    private static readonly List<AuthorDto> _authors = new()
    {
        new AuthorDto { Id = 1, Name = "John Doe", Email = "john.doe@example.com", CreatedAt = DateTime.UtcNow },
        new AuthorDto { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", CreatedAt = DateTime.UtcNow }
    };

    public Task<AuthorDto?> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = _authors.FirstOrDefault(a => a.Id == request.Id);
        return Task.FromResult(author);
    }
}
