using MediatR;

namespace WebApi.Features.Books.Commands;

public record UpdateBookCommand(int Id, UpdateBookRequest Request) : IRequest;

public class UpdateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AuthorId { get; set; }
}

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand>
{
    public Task<Unit> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        // This would typically update the book in a database
        // For in-memory implementation, we'll just simulate success
        return Task.FromResult(Unit.Value);
    }
}
