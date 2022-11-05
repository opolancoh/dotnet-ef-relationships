using EntityFrameworkRelationships.Web.Contracts;
using EntityFrameworkRelationships.Web.Data;
using EntityFrameworkRelationships.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkRelationships.Web.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }


    public static void ConfigurePersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IReviewService, ReviewService>();
    }

    public static void ConfigureSqlContext(this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));
}