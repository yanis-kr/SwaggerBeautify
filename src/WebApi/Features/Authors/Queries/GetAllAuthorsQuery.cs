using MediatR;
using WebApi.Models.Authors;

namespace WebApi.Features.Authors.Queries;

public record GetAllAuthorsQuery : IRequest<IEnumerable<Author>>;


