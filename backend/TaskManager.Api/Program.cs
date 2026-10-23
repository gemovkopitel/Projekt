using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using TaskManager.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Lokalne ustawienia (connection string do PostgreSQL z hasłem) – plik nie trafia do repozytorium.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("TaskManagerDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Brak connection stringa 'ConnectionStrings:TaskManagerDb'. " +
        "Skopiuj appsettings.Local.example.json do appsettings.Local.json i wpisz dane logowania do PostgreSQL.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, sql => sql.EnableRetryOnFailure()));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Frontend jest serwowany z innego adresu (np. Live Server), więc potrzebny jest CORS.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // W trybie deweloperskim API serwuje też frontend: http://localhost:5113/
    var frontendPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "..", "frontend"));
    if (Directory.Exists(frontendPath))
    {
        var frontendFiles = new PhysicalFileProvider(frontendPath);
        app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = frontendFiles });
        app.UseStaticFiles(new StaticFileOptions { FileProvider = frontendFiles });
    }
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
