using WebApi.Infrastructure.Mediator;
using WebApi.Features.Books.Models;
using WebApi.Models;

namespace WebApi.Features.Books.Commands;

public record CreateBookCommand(CreateBookRequest Request, CommonParameters CommonParams) : IRequest<BookDto>;

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
        // Access common parameters if needed
        // var correlationId = request.CommonParams.CorrelationId ?? Guid.NewGuid();
        // var userContext = request.CommonParams.UserContext;
        
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
