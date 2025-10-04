using MediatR;
using WebApi.Models.Authors;

namespace WebApi.Features.Authors.Commands;

public record UpdateAuthorCommand(int Id, UpdateAuthorRequest Request) : IRequest;


