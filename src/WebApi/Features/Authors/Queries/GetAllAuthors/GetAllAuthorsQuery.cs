using MediatR;
using WebApi.Features.Authors.Models;

namespace WebApi.Features.Authors.Queries.GetAllAuthors;

public record GetAllAuthorsQuery : IRequest<IEnumerable<AuthorDto>>;