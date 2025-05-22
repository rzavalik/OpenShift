namespace JokePresentation;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add environment variables and other services
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpClient();
        builder.Services.AddHealthChecks();
        builder.WebHost.UseUrls(Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://*:8080");

        WebApplication app = builder.Build();

        // Middleware for error handling and HSTS (HTTP Strict Transport Security)
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // Middleware for HTTP redirection and routing
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        // Define the route for MVC Controllers
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // Define health route
        app.MapHealthChecks("/health");

        // Run the application
        app.Run();
    }
}
