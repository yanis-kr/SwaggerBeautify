using MediatR;
using WebApi.Models.Authors;

namespace WebApi.Features.Authors.Commands;

public record CreateAuthorCommand(CreateAuthorRequest Request) : IRequest<Author>;

