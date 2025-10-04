using MediatR;
using WebApi.Features.Books.Models;

namespace WebApi.Features.Books.Queries.GetBooksByAuthor;

public record GetBooksByAuthorQuery(int AuthorId) : IRequest<IEnumerable<BookDto>>;