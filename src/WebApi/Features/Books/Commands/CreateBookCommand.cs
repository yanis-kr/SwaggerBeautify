using MediatR;
using WebApi.Models.Books;

namespace WebApi.Features.Books.Commands;

public record CreateBookCommand(CreateBookRequest Request) : IRequest<Book>;

