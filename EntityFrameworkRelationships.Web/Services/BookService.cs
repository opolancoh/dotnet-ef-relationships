using Microsoft.EntityFrameworkCore;
using EntityFrameworkRelationships.Web.Contracts;
using EntityFrameworkRelationships.Web.Data;
using EntityFrameworkRelationships.Web.DTOs;
using EntityFrameworkRelationships.Web.Exceptions;
using EntityFrameworkRelationships.Web.Models;

namespace EntityFrameworkRelationships.Web.Services;

public class BookService : IBookService
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Book> _entity;

    public BookService(ApplicationDbContext context)
    {
        _context = context;
        _entity = context.Books;
    }

    public async Task<IEnumerable<BookDetailDto>> GetAll()
    {
        var query = GetBookDetailDtoQuery();

        return await query.ToListAsync();
    }

    public async Task<BookDetailDto?> GetById(Guid id)
    {
        var query = GetBookDetailDtoQuery();

        return await query.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<BookDto> Create(BookForCreatingDto item)
    {
        var newItem = new Book()
        {
            Id = new Guid(),
            Title = item.Title!,
            PublishedOn = item.PublishedOn!.Value.ToUniversalTime(),
            Image = new BookImage
            {
                Url = item.Image!.Url,
                Alt = item.Image!.Alt
            }
        };

        newItem.AuthorsLink = new List<BookAuthor>();
        foreach (var authorId in item.Authors)
        {
            newItem.AuthorsLink.Add(new BookAuthor {BookId = newItem.Id, AuthorId = authorId});
        }

        _entity.Add(newItem);
        await _context.SaveChangesAsync();

        var dto = TransformItemToBookDto(newItem);

        return dto;
    }

    public async Task<BookDto> Update(Guid id, BookForUpdatingDto item)
    {
        var currentItem = await _entity
            .Include(x => x.Image)
            .Include(x => x.AuthorsLink)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (currentItem == null)
        {
            throw new EntityNotFoundException(id);
        }

        // Main update
        currentItem.Title = item.Title!;
        currentItem.PublishedOn = item.PublishedOn!.Value.ToUniversalTime();

        // Image update
        currentItem.Image.Url = item.Image!.Url;
        currentItem.Image.Alt = item.Image!.Alt;

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
        await _context.SaveChangesAsync();

        var dto = TransformItemToBookDto(currentItem);

        return dto;
    }

    public async Task Remove(Guid id)
    {
        var item = new Book {Id = id};

        _entity.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ItemExists(id))
            {
                throw new EntityNotFoundException(id);
            }
            else
            {
                throw;
            }
        }
    }

    private bool ItemExists(Guid id)
    {
        return _entity.Any(e => e.Id == id);
    }

    private static BookDetailDto GetBookDetailDto(Book item)
    {
        return new BookDetailDto
        {
            Id = item.Id,
            Title = item.Title,
            PublishedOn = item.PublishedOn,
            Image = new BookImageDto
            {
                Url = item.Image.Url,
                Alt = item.Image.Alt
            },
            Authors = item.AuthorsLink
                .Select(y => new AuthorPlaneDto
                {
                    Id = y.Author.Id,
                    Name = y.Author.Name
                })
        };
    }

    private static BookDto TransformItemToBookDto(Book item)
    {
        return new BookDto
        {
            Id = item.Id,
            Title = item.Title,
            PublishedOn = item.PublishedOn,
            Image = new BookImageDto
            {
                Url = item.Image.Url,
                Alt = item.Image.Alt
            },
            Authors = item.AuthorsLink.Select(x => x.AuthorId)
        };
    }

    private IQueryable<BookDetailDto> GetBookDetailDtoQuery()
    {
        return _entity
            .AsNoTracking()
            .Select(x => new BookDetailDto
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
                    .Select(y => new AuthorPlaneDto
                    {
                        Id = y.Author.Id,
                        Name = y.Author.Name
                    })
            });
    }
}