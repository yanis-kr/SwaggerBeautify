using MediatR;
using WebApi.Features.Books.Models;

namespace WebApi.Features.Books.Commands.CreateBook;

public record CreateBookCommand(CreateBookRequest Request) : IRequest<BookDto>;