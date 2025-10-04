using MediatR;

namespace WebApi.Features.Books.Commands.UpdateBook;

public record UpdateBookCommand(int Id, UpdateBookRequest Request) : IRequest;