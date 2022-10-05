using EntityFrameworkRelationships.Data;
using EntityFrameworkRelationships.DTOs;
using EntityFrameworkRelationships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkRelationships.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorsController : ControllerBase
{
    private readonly BookContext _context;
    private readonly DbSet<Author> _dbSet;

    public AuthorsController(BookContext context)
    {
        _context = context;
        _dbSet = context.Authors;
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> Add(AuthorDto item)
    {
        var newItem = new Author()
        {
            Id = new Guid(),
            Name = item.Name,
            Email = item.Email
        };

        _dbSet.Add(newItem);
        await _context.SaveChangesAsync();

        var dto = new AuthorDto()
        {
            Id = newItem.Id,
            Name = newItem.Name,
            Email = newItem.Email
        };

        return CreatedAtAction(nameof(GetById), new {id = dto.Id}, dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> Get()
    {
        return await _dbSet
            .AsNoTracking()
            .Select(x => new AuthorDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email
            })
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetById(Guid id)
    {
        var item = await _dbSet
            .AsNoTracking()
            .Select(x => new AuthorDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email
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
    public async Task<IActionResult> Update(Guid id, AuthorDto item)
    {
        var updateItem = new Author()
        {
            Id = id,
            Name = item.Name,
            Email = item.Email
        };

        _context.Entry(updateItem).State = EntityState.Modified;

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