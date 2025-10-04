using MediatR;

namespace WebApi.Features.Books.Commands.DeleteBook;

public class DeleteBookHandler : IRequestHandler<DeleteBookCommand>
{
    public Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        // This would typically delete the book from a database
        // For in-memory implementation, we'll just simulate success
        return Task.FromResult(Unit.Value);
    }
}