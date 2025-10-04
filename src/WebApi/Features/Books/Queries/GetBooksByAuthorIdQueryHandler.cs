using MediatR;
using WebApi.Models.Books;

namespace WebApi.Features.Books.Queries;

public class GetBooksByAuthorIdQueryHandler : IRequestHandler<GetBooksByAuthorIdQuery, IEnumerable<Book>>
{
    private static readonly List<Book> _books = new()
    {
        new Book { Id = 1, Title = "The Great Book", Description = "A wonderful book", AuthorId = 1, CreatedAt = DateTime.UtcNow },
        new Book { Id = 2, Title = "Another Great Book", Description = "Another wonderful book", AuthorId = 2, CreatedAt = DateTime.UtcNow }
    };

    public Task<IEnumerable<Book>> Handle(GetBooksByAuthorIdQuery request, CancellationToken cancellationToken)
    {
        var books = _books.Where(b => b.AuthorId == request.AuthorId);
        return Task.FromResult(books);
    }
}


