using EntityFrameworkRelationships.Data;
using EntityFrameworkRelationships.DTOs;
using EntityFrameworkRelationships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EntityFrameworkRelationships.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly BookContext _context;
    private readonly DbSet<Book> _dbSet;

    public BooksController(BookContext context)
    {
        _context = context;
        _dbSet = context.Books;
    }

    [HttpPost]
    public async Task<ActionResult<BookAddUpdateResponse>> Add(BookAddUpdateRequest item)
    {
        var newItem = new Book()
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

        foreach (var authorId in item.Authors)
        {
            newItem.AuthorsLink.Add(new BookAuthor {BookId = newItem.Id, AuthorId = authorId});
        }

        _dbSet.Add(newItem);
        await _context.SaveChangesAsync();

        var dto = new BookAddUpdateResponse
        {
            Id = newItem.Id,
            Title = newItem.Title,
            PublishedOn = newItem.PublishedOn,
            Image = new BookImageDto
            {
                Url = newItem.Image.Url,
                Alt = newItem.Image.Alt
            },
            Authors = newItem.AuthorsLink.Select(x => x.AuthorId)
        };

        return CreatedAtAction(nameof(GetById), new {id = dto.Id}, dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> Get()
    {
        return await _dbSet
            .AsNoTracking()
            .Select(x => new BookDto
            {
                Id = x.Id,
                Title = x.Title,
                PublishedOn = x.PublishedOn,
                Image = new BookImageDto
                {
                    Url = x.Image.Url,
                    Alt = x.Image.Alt
                },
                Authors = x.AuthorsLink
                    .Select(y => new AuthorLiteDto
                    {
                        Id = y.Author.Id,
                        Name = y.Author.Name
                    })
            })
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(Guid id)
    {
        var item = await _dbSet
            .AsNoTracking()
            .Select(x => new BookDto
            {
                Id = x.Id,
                Title = x.Title,
                PublishedOn = x.PublishedOn,
                Image = new BookImageDto
                {
                    Url = x.Image.Url,
                    Alt = x.Image.Alt
                },
                Authors = x.AuthorsLink
                    .Select(y => new AuthorLiteDto
                    {
                        Id = y.Author.Id,
                        Name = y.Author.Name
                    })
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
        var item = await _dbSet
            .Include(x => x.AuthorsLink)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
        {
            return NotFound();
        }

        _dbSet.Remove(item);
        Console.WriteLine(_context.ChangeTracker.DebugView.LongView);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, BookAddUpdateRequest item)
    {
        var currentItem = await _dbSet
            .Include(x => x.Image)
            .Include(x => x.AuthorsLink)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (currentItem == null)
        {
            return NotFound();
        }

        // Main update
        currentItem.Title = item.Title;
        currentItem.PublishedOn = item.PublishedOn;

        // Image update
        currentItem.Image.Url = item.Image.Url;
        currentItem.Image.Alt = item.Image.Alt;

        // Authors update
        var authorsToAdd = item.Authors
            .Where(x => currentItem.AuthorsLink.All(y => y.AuthorId != x))
            .Select(x => new BookAuthor {BookId = id, AuthorId = x});
        foreach (var authorToAdd in authorsToAdd)
        {
            currentItem.AuthorsLink.Add(authorToAdd);
        }

        var authorsToRemove = currentItem.AuthorsLink
            .Where(x => item.Authors.All(y => y != x.AuthorId))
            .ToList();
        foreach (var authorToRemove in authorsToRemove)
        {
            currentItem.AuthorsLink.Remove(authorToRemove);
        }

        _context.Entry(currentItem).State = EntityState.Modified;
        Console.WriteLine(_context.ChangeTracker.DebugView.LongView);

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