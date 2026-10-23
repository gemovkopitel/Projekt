using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Data;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(AppDbContext db) : ControllerBase
{
    // GET /api/health – czy API działa
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok" });

    // GET /api/health/db – czy jest połączenie z bazą danych
    [HttpGet("db")]
    public async Task<IActionResult> Database()
    {
        var canConnect = await db.Database.CanConnectAsync();
        return canConnect
            ? Ok(new { database = "ok" })
            : StatusCode(StatusCodes.Status503ServiceUnavailable, new { database = "unavailable" });
    }
}
