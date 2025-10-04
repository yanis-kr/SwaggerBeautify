using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.Books.Commands;
using WebApi.Features.Books.Queries;
using WebApi.Models.Books;

namespace WebApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BooksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Book>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var books = await mediator.Send(new GetAllBooksQuery());
        return Ok(books);
    }

    [HttpGet("{authorId:int}")]
    [ProducesResponseType(typeof(IEnumerable<Book>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAuthorId(int authorId)
    {
        var books = await mediator.Send(new GetBooksByAuthorIdQuery(authorId));
        return Ok(books);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateBookRequest request)
    {
        var book = await mediator.Send(new CreateBookCommand(request));
        return CreatedAtAction(nameof(GetAll), new { }, book);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateBookRequest request)
    {
        await mediator.Send(new UpdateBookCommand(id, request));
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteBookCommand(id));
        return NoContent();
    }
}
