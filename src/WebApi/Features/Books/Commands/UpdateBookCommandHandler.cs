using MediatR;

namespace WebApi.Features.Books.Commands;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand>
{
    public Task<Unit> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        // This would typically update the book in a database
        // For in-memory implementation, we'll just simulate success
        return Task.FromResult(Unit.Value);
    }
}
