using MediatR;
using WebApi.Models.Books;

namespace WebApi.Features.Books.Queries;

public record GetAllBooksQuery : IRequest<IEnumerable<Book>>;

