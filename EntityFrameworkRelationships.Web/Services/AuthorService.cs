using Microsoft.EntityFrameworkCore;
using EntityFrameworkRelationships.Web.Contracts;
using EntityFrameworkRelationships.Web.Data;
using EntityFrameworkRelationships.Web.DTOs;
using EntityFrameworkRelationships.Web.Exceptions;
using EntityFrameworkRelationships.Web.Models;

namespace EntityFrameworkRelationships.Web.Services;

public class AuthorService : IAuthorService
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Author> _entity;

    public AuthorService(ApplicationDbContext context)
    {
        _context = context;
        _entity = context.Authors;
    }

    public async Task<IEnumerable<AuthorDto>> GetAll()
    {
        var query = GetAuthorDtoQuery();

        return await query.ToListAsync();
    }

    public async Task<AuthorDto?> GetById(Guid id)
    {
        var query = GetAuthorDtoQuery();

        return await query.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<AuthorDto> Create(AuthorForCreatingUpdatingDto item)
    {
        var newItem = new Author()
        {
            Id = new Guid(),
            Name = item.Name,
            Email = item.Email
        };

        _entity.Add(newItem);
        await _context.SaveChangesAsync();

        var dto = TransformItemToAuthorDto(newItem);

        return dto;
    }

    public async Task<AuthorDto> Update(Guid id, AuthorForCreatingUpdatingDto item)
    {
        var itemToUpdate = new Author()
        {
            Id = id,
            Name = item.Name,
            Email = item.Email
        };

        _context.Entry(itemToUpdate).State = EntityState.Modified;

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

        var dto = TransformItemToAuthorDto(itemToUpdate);

        return dto;
    }

    public async Task Remove(Guid id)
    {
        var item = new Author {Id = id};

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

    private IQueryable<AuthorDto> GetAuthorDtoQuery()
    {
        return _entity
            .AsNoTracking()
            .Select(x => new AuthorDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email
            });
    }
    
    private static AuthorDto TransformItemToAuthorDto(Author item)
    {
        return new AuthorDto
        {
            Id = item.Id,
            Name = item.Name,
            Email = item.Email,
        };
    }
}