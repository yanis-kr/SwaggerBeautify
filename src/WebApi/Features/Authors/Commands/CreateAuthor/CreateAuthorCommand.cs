using MediatR;
using WebApi.Features.Authors.Models;

namespace WebApi.Features.Authors.Commands.CreateAuthor;

public record CreateAuthorCommand(CreateAuthorRequest Request) : IRequest<AuthorDto>;