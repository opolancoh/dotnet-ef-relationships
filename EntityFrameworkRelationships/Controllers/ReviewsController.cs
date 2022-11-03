using EntityFrameworkRelationships.Data;
using EntityFrameworkRelationships.DTOs;
using EntityFrameworkRelationships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkRelationships.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly BookContext _context;
    private readonly DbSet<Review> _dbSet;

    public ReviewsController(BookContext context)
    {
        _context = context;
        _dbSet = context.Reviews;
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Add(ReviewDto item)
    {
        var newItem = new Review()
        {
            Id = new Guid(),
            Comment = item.Comment,
            Rating = item.Rating,
            BookId = item.BookId
        };

        _dbSet.Add(newItem);
        await _context.SaveChangesAsync();

        var dto = new ReviewDto
        {
            Id = newItem.Id,
            Comment = newItem.Comment,
            Rating = newItem.Rating,
            BookId = newItem.BookId
        };

        return CreatedAtAction(nameof(GetById), new {id = dto.Id}, dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> Get()
    {
        return await _dbSet
            .AsNoTracking()
            .Select(x => new ReviewDto
            {
                Id = x.Id,
                Comment = x.Comment,
                Rating = x.Rating,
                BookId = x.BookId
            })
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetById(Guid id)
    {
        var item = await _dbSet
            .AsNoTracking()
            .Select(x => new ReviewDto
            {
                Id = x.Id,
                Comment = x.Comment,
                Rating = x.Rating,
                BookId = x.BookId
            })
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var item = await _dbSet.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        _dbSet.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ReviewDto item)
    {
        var updateItem = new Review
        {
            Id = id,
            Comment = item.Comment,
            Rating = item.Rating
        };

        // Partial update
        _context.Entry(updateItem).Property(x => x.Comment).IsModified = true;
        _context.Entry(updateItem).Property(x => x.Rating).IsModified = true;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ItemExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    private bool ItemExists(Guid id)
    {
        return _dbSet.Any(e => e.Id == id);
    }
}