using WebApi.Infrastructure.Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Features.Authors.Commands;
using WebApi.Features.Authors.Queries;
using WebApi.Features.Authors.Models;
using WebApi.Features.Books.Models;

namespace WebApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthorsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets an author by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the author (e.g., 1)</param>
    /// <returns>Returns the author details if found</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get author by ID",
        Description = "Retrieves a specific author using their unique identifier"
    )]
    public async Task<IActionResult> Get(int id)
    {
        var author = await mediator.Send(new GetAuthorByIdQuery(id));
        if (author == null)
            return NotFound();
        
        return Ok(author);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuthorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var authors = await mediator.Send(new GetAllAuthorsQuery());
        return Ok(authors);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateAuthorRequest request)
    {
        var author = await mediator.Send(new CreateAuthorCommand(request));
        return CreatedAtAction(nameof(Get), new { id = author.Id }, author);
    }

    /// <summary>
    /// Updates an existing author
    /// </summary>
    /// <param name="id">The unique identifier of the author to update (e.g., 1)</param>
    /// <param name="request">The updated author information</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update author",
        Description = "Updates an existing author's information"
    )]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateAuthorRequest request)
    {
        await mediator.Send(new UpdateAuthorCommand(id, request));
        return NoContent();
    }

    /// <summary>
    /// Deletes an author by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the author to delete (e.g., 1)</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Delete author",
        Description = "Removes an author from the system"
    )]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteAuthorCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Gets all books written by a specific author
    /// </summary>
    /// <param name="id">The unique identifier of the author (e.g., 1)</param>
    /// <returns>Returns a list of books written by the author</returns>
    [HttpGet("{id:int}/books")]
    [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get books by author",
        Description = "Retrieves all books written by a specific author"
    )]
    public async Task<IActionResult> GetBooks(int id)
    {
        var books = await mediator.Send(new GetBooksByAuthorQuery(id));
        return Ok(books);
    }
}
