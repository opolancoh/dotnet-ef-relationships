using EntityFrameworkRelationships.Data;
using EntityFrameworkRelationships.DTOs;
using EntityFrameworkRelationships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkRelationships.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly BookContext _context;

    public BooksController(BookContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> Get()
    {
        return await _context.Books
            .AsNoTracking()
            .Include(x => x.Image)
            .Select(x => new BookDto
            {
                Id = x.Id,
                Title = x.Title,
                PublishedOn = x.PublishedOn,
                Image = new BookImageForAddUpdateDto
                {
                    Url = x.Image.Url,
                    Alt = x.Image.Alt
                }
            })
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(Guid id)
    {
        var item = await _context.Books
            .AsNoTracking()
            .Include(x => x.Image)
            .Select(x => new BookDto
            {
                Id = x.Id,
                Title = x.Title,
                PublishedOn = x.PublishedOn,
                Image = new BookImageForAddUpdateDto
                {
                    Url = x.Image.Url,
                    Alt = x.Image.Alt
                }
            })
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, BookForAddUpdateDto item)
    {
        var book = new Book
        {
            Id = id,
            Title = item.Title,
            PublishedOn = item.PublishedOn,
        };
        var image = new BookImage
        {
            BookId = id,
            Url = item.Image.Url,
            Alt = item.Image.Alt
        };

        _context.Entry(book).State = EntityState.Modified;
        _context.Entry(image).State = EntityState.Modified;

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
    
    [HttpPost]
    public async Task<ActionResult<BookDto>> Add(BookForAddUpdateDto item)
    {
        var book = new Book
        {
            Id = new Guid(),
            Title = item.Title,
            PublishedOn = item.PublishedOn,
            Image = new BookImage
            {
                Url = item.Image.Url,
                Alt = item.Image.Alt
            }
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var dto = new BookDto()
        {
            Id = book.Id,
            Title = book.Title,
            PublishedOn = book.PublishedOn,
            Image = new BookImageForAddUpdateDto
            {
                Url = book.Image.Url,
                Alt = book.Image.Alt
            }
        };

        return CreatedAtAction(nameof(GetById), new {id = dto.Id}, dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var item = await _context.Books.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        _context.Books.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ItemExists(Guid id)
    {
        return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}