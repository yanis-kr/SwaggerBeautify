using MediatR;

namespace WebApi.Features.Books.Commands.DeleteBook;

public record DeleteBookCommand(int Id) : IRequest;