using WebApi.Infrastructure.Mediator;
using WebApi.Features.Books.Models;

namespace WebApi.Features.Books.Commands;

public record CreateBookCommand(CreateBookRequest Request) : IRequest<BookDto>;

public class CreateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AuthorId { get; set; }
}

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
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
