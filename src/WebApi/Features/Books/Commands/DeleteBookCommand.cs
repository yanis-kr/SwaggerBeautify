using WebApi.Infrastructure.Mediator;
using WebApi.Models;

namespace WebApi.Features.Books.Commands;

public record DeleteBookCommand(int Id, CommonParameters CommonParams) : IRequest;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    public Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        // This would typically delete the book from a database
        // For in-memory implementation, we'll just simulate success
        return Task.FromResult(Unit.Value);
    }
}
