using MediatR;
using WebApi.Features.Books.Models;

namespace WebApi.Features.Books.Queries.GetAllBooks;

public record GetAllBooksQuery : IRequest<IEnumerable<BookDto>>;