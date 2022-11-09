using Microsoft.AspNetCore.Mvc;
using EntityFrameworkRelationships.Web.Contracts;
using EntityFrameworkRelationships.Web.DTOs;
using EntityFrameworkRelationships.Web.Exceptions;

namespace EntityFrameworkRelationships.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;

    public BooksController(IBookService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDetailDto>>> GetAll()
    {
        var items = await _service.GetAll();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDetailDto>> GetById(Guid id)
    {
        var item = await _service.GetById(id);

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create(BookForCreatingDto item)
    {
        var newItem = await _service.Create(item);

        return CreatedAtAction(nameof(GetById), new {id = newItem.Id}, newItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, BookForUpdatingDto item)
    {
        try
        {
            await _service.Update(id, item);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        try
        {
            await _service.Remove(id);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}