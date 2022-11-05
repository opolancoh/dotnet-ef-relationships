using EntityFrameworkRelationships.Tests.Helpers;
using EntityFrameworkRelationships.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkRelationships.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove current DbContext
            var serviceDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (serviceDescriptor != null) services.Remove(serviceDescriptor);

            // Add DbContext for testing
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseNpgsql("Server=localhost; Database=books_db_test; Username=postgres; Password=My@Passw0rd;"));

            //
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Don't update/remove this initial data
            var authors = DbHelper.Authors;
            db.Authors?.AddRange(authors);
            db.SaveChanges();

            var books = DbHelper.Books;
            db.Books?.AddRange(books);
            db.SaveChanges();

            var reviews = DbHelper.Reviews;
            db.Reviews?.AddRange(reviews);
            db.SaveChanges();

            logger.LogError("All data was saved successfully");
        });
    }
}