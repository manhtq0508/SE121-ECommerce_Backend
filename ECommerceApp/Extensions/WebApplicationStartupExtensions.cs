using ECommerceApp.Data;
using ECommerceApp.Middlewares;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

namespace ECommerceApp.Extensions;

public static class WebApplicationStartupExtensions
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static WebApplication UseApplicationPipeline(this WebApplication app, string bearerSecurityScheme)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Title = "ECommerce API";
                options.AddPreferredSecuritySchemes([bearerSecurityScheme]);
                options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
            });
        }

        app.UseLoggingMiddleware();
        app.UseErrorHandlingMiddleware();
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
