using MediatR;

namespace WebApi.Features.Authors.Commands;

public record DeleteAuthorCommand(int Id) : IRequest;

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand>
{
    public Task<Unit> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        // This would typically delete the author from a database
        // For in-memory implementation, we'll just simulate success
        return Task.FromResult(Unit.Value);
    }
}
