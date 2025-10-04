using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.Authors.Commands;
using WebApi.Features.Authors.Queries;
using WebApi.Models.Authors;

namespace WebApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthorsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Author), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var author = await mediator.Send(new GetAuthorByIdQuery(id));
        if (author == null)
            return NotFound();
        
        return Ok(author);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Author>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var authors = await mediator.Send(new GetAllAuthorsQuery());
        return Ok(authors);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Author), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateAuthorRequest request)
    {
        var author = await mediator.Send(new CreateAuthorCommand(request));
        return CreatedAtAction(nameof(Get), new { id = author.Id }, author);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateAuthorRequest request)
    {
        await mediator.Send(new UpdateAuthorCommand(id, request));
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteAuthorCommand(id));
        return NoContent();
    }
}
