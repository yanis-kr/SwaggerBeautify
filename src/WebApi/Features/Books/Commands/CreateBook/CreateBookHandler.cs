using MediatR;
using WebApi.Features.Books.Models;

namespace WebApi.Features.Books.Commands.CreateBook;

public class CreateBookHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private static readonly List<BookDto> _books = new();
    private static int _nextId = 1;

    public Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = new BookDto
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