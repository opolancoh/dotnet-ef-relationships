using Microsoft.AspNetCore.Mvc;
using EntityFrameworkRelationships.Web.Contracts;
using EntityFrameworkRelationships.Web.DTOs;
using EntityFrameworkRelationships.Web.Exceptions;

namespace EntityFrameworkRelationships.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _service;

    public ReviewsController(IReviewService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAll()
    {
        var items = await _service.GetAll();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetById(Guid id)
    {
        var item = await _service.GetById(id);

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create(ReviewForCreatingDto item)
    {
        var newItem = await _service.Create(item);

        return CreatedAtAction(nameof(GetById), new {id = newItem.Id}, newItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ReviewForUpdatingDto item)
    {
        if (id != item.Id)
            return BadRequest();

        try
        {
            await _service.Update(item);
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