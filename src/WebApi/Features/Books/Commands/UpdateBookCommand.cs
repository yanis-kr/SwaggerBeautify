using MediatR;
using WebApi.Models.Books;

namespace WebApi.Features.Books.Commands;

public record UpdateBookCommand(int Id, UpdateBookRequest Request) : IRequest;

