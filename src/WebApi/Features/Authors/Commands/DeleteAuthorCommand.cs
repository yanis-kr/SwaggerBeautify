using MediatR;

namespace WebApi.Features.Authors.Commands;

public record DeleteAuthorCommand(int Id) : IRequest;


