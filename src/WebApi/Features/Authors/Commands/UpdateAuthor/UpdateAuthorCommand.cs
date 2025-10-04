using MediatR;

namespace WebApi.Features.Authors.Commands.UpdateAuthor;

public record UpdateAuthorCommand(int Id, UpdateAuthorRequest Request) : IRequest;