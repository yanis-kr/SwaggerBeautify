using MediatR;

namespace WebApi.Features.Authors.Commands.UpdateAuthor;

public class UpdateAuthorHandler : IRequestHandler<UpdateAuthorCommand>
{
    public Task<Unit> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        // This would typically update the author in a database
        // For in-memory implementation, we'll just simulate success
        return Task.FromResult(Unit.Value);
    }
}