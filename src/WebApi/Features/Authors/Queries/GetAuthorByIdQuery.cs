using MediatR;
using WebApi.Models.Authors;

namespace WebApi.Features.Authors.Queries;

public record GetAuthorByIdQuery(int Id) : IRequest<Author?>;

