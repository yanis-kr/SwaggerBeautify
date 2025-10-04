using MediatR;
using WebApi.Models.Books;

namespace WebApi.Features.Books.Queries;

public record GetBooksByAuthorIdQuery(int AuthorId) : IRequest<IEnumerable<Book>>;

