using MediatR;
using WebApi.Features.Authors.Models;

namespace WebApi.Features.Authors.Queries.GetAuthorById;

public record GetAuthorByIdQuery(int Id) : IRequest<AuthorDto?>;