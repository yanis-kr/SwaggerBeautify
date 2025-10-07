using WebApi.Infrastructure.Mediator;
using WebApi.Features.Books.Models;
using WebApi.Models;

namespace WebApi.Features.Books.Queries;

public record GetBookByIdQuery(int Id, CommonParameters CommonParams) : IRequest<BookDto?>;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private static readonly List<BookDto> _books = new()
    {
        new BookDto { Id = 1, Title = "The Great Book", Description = "A wonderful book", AuthorId = 1, CreatedAt = DateTime.UtcNow },
        new BookDto { Id = 2, Title = "Another Great Book", Description = "Another wonderful book", AuthorId = 2, CreatedAt = DateTime.UtcNow }
    };

    public Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = _books.FirstOrDefault(b => b.Id == request.Id);
        return Task.FromResult(book);
    }
}
