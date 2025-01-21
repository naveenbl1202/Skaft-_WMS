using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkaftoBageriA.Data;
using SkaftoBageriA.Models;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
    .EnableSensitiveDataLogging() // Useful for debugging, remove in production
    .LogTo(Console.WriteLine));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false; // No special characters required
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add MVC and logging
builder.Services.AddControllersWithViews();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add custom database logger filter for detailed EF logs
DiagnosticListener.AllListeners.Subscribe(new DatabaseLoggerFilter());

var app = builder.Build();

// Apply migrations and ensure the database is up to date
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations.");
    }
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed roles and admin user
await SeedRolesAndAdminUserAsync(app.Services);

app.Run();

/// <summary>
/// Seeds roles and a default admin user.
/// </summary>
async Task SeedRolesAndAdminUserAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var serviceProvider = scope.ServiceProvider;

    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var roles = new[] { "Admin", "User" };

        // Ensure roles exist
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    logger.LogInformation($"Role '{role}' created successfully.");
                }
                else
                {
                    logger.LogError($"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogInformation($"Role '{role}' already exists.");
            }
        }

        // Seed default admin user
        var adminEmail = builder.Configuration["AdminUser:Email"] ?? "admin@skafto.com";
        var adminPassword = builder.Configuration["AdminUser:Password"] ?? "Admin123!";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "Administrator"
            };

            var result = await userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
                logger.LogInformation($"Admin user '{adminEmail}' created and added to 'Admin' role.");
            }
            else
            {
                logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            logger.LogInformation($"Admin user '{adminEmail}' already exists.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError($"An error occurred during role or admin user initialization: {ex.Message}");
    }
}

/// <summary>
/// Custom logger filter for Entity Framework diagnostics.
/// </summary>
public class DatabaseLoggerFilter : IObserver<DiagnosticListener>
{
    public void OnNext(DiagnosticListener value)
    {
        if (value.Name == "Microsoft.EntityFrameworkCore")
        {
            value.Subscribe(new DatabaseObserver());
        }
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }
}

public class DatabaseObserver : IObserver<KeyValuePair<string, object>>
{
    public void OnNext(KeyValuePair<string, object> value)
    {
        Console.WriteLine($"EF LOG: {value.Key} => {value.Value}");
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }
}
