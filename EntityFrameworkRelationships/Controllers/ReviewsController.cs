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

    public ReviewsController(BookContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Add(ReviewDto item)
    {
        var review = new Review()
        {
            Id = new Guid(),
            Comment = item.Comment,
            Rating = item.Rating,
            BookId = item.BookId
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var dto = new ReviewDto()
        {
            Id = review.Id,
            Comment = review.Comment,
            Rating = review.Rating,
            BookId = review.BookId
        };

        return CreatedAtAction(nameof(GetById), new {id = dto.Id}, dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> Get()
    {
        return await _context.Reviews
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
        var item = await _context.Reviews
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
        var item = await _context.Reviews.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        _context.Reviews.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ReviewDto item)
    {
        var review = new Review
        {
            Id = id,
            Comment = item.Comment,
            Rating = item.Rating
        };

        // Partial update
        _context.Entry(review).Property(x => x.Comment).IsModified = true;
        _context.Entry(review).Property(x => x.Rating).IsModified = true;

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
        return (_context.Reviews?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}