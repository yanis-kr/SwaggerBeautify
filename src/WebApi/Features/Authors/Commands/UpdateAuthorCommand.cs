using MediatR;

namespace WebApi.Features.Authors.Commands;

public record UpdateAuthorCommand(int Id, UpdateAuthorRequest Request) : IRequest;

public class UpdateAuthorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand>
{
    public Task<Unit> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        // This would typically update the author in a database
        // For in-memory implementation, we'll just simulate success
        return Task.FromResult(Unit.Value);
    }
}
