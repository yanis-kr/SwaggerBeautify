using MediatR;

namespace WebApi.Features.Authors.Commands.DeleteAuthor;

public record DeleteAuthorCommand(int Id) : IRequest;