using EntityFrameworkRelationships.Web.Models;

namespace EntityFrameworkRelationships.Tests.Helpers;

public static class Utilities
{
    public static List<Author> GetAuthors()
    {
        return new List<Author>
        {
            new Author
            {
                Id = Guid.NewGuid(),
                Name = "Sheldon Cooper",
                Email = "Sheldon.Cooper@email.com"
            },
            new Author
            {
                Id = Guid.NewGuid(),
                Name = "Marie Curie",
                Email = "Marie.Curie@email.com"
            },
            new Author
            {
                Id = Guid.NewGuid(),
                Name = "Mary Shelley",
                Email = "Mary.Shelley@email.com"
            }
        };
    }
}