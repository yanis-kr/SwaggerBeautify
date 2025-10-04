using MediatR;

namespace WebApi.Features.Books.Commands;

public record DeleteBookCommand(int Id) : IRequest;

