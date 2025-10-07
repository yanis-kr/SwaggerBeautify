using WebApi.Infrastructure.Mediator;
using WebApi.Features.Books.Models;
using WebApi.Models;

namespace WebApi.Features.Books.Queries;

public record GetAllBooksQuery(CommonParameters CommonParams) : IRequest<IEnumerable<BookDto>>;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookDto>>
{
    private static readonly List<BookDto> _books = new()
    {
        new BookDto { Id = 1, Title = "The Great Book", Description = "A wonderful book", AuthorId = 1, CreatedAt = DateTime.UtcNow },
        new BookDto { Id = 2, Title = "Another Great Book", Description = "Another wonderful book", AuthorId = 2, CreatedAt = DateTime.UtcNow }
    };

    public Task<IEnumerable<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_books.AsEnumerable());
    }
}
