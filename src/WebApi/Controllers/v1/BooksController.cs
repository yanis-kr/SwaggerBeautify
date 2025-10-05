using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Features.Books.Commands;
using WebApi.Features.Books.Queries;
using WebApi.Features.Books.Models;

namespace WebApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BooksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var books = await mediator.Send(new GetAllBooksQuery());
        return Ok(books);
    }

    /// <summary>
    /// Gets all books written by a specific author
    /// </summary>
    /// <param name="authorId">The unique identifier of the author (e.g., 42)</param>
    /// <returns>Returns a list of books written by the author</returns>
    [HttpGet("{authorId:int}")]
    [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get books by author ID",
        Description = "Retrieves all books written by a specific author using the author's ID"
    )]
    public async Task<IActionResult> GetByAuthorId(int authorId)
    {
        var books = await mediator.Send(new GetBooksByAuthorQuery(authorId));
        return Ok(books);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateBookRequest request)
    {
        var book = await mediator.Send(new CreateBookCommand(request));
        return CreatedAtAction(nameof(GetAll), new { }, book);
    }

    /// <summary>
    /// Updates an existing book
    /// </summary>
    /// <param name="id">The unique identifier of the book to update (e.g., 101)</param>
    /// <param name="request">The updated book information</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update book",
        Description = "Updates an existing book's information"
    )]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateBookRequest request)
    {
        await mediator.Send(new UpdateBookCommand(id, request));
        return NoContent();
    }

    /// <summary>
    /// Deletes a book by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the book to delete (e.g., 101)</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Delete book",
        Description = "Removes a book from the system"
    )]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteBookCommand(id));
        return NoContent();
    }
}
