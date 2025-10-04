using MediatR;
using WebApi.Models.Books;

namespace WebApi.Features.Books.Commands;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Book>
{
    private static readonly List<Book> _books = new();
    private static int _nextId = 1;

    public Task<Book> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = new Book
        {
            Id = _nextId++,
            Title = request.Request.Title,
            Description = request.Request.Description,
            AuthorId = request.Request.AuthorId,
            CreatedAt = DateTime.UtcNow
        };

        _books.Add(book);
        return Task.FromResult(book);
    }
}


