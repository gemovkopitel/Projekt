using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Dtos;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(AppDbContext db) : ControllerBase
{
    // GET /api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await db.Users.AsNoTracking().OrderBy(u => u.Id).ToListAsync();
        return Ok(users.Select(UserDto.From));
    }

    // GET /api/users/2
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return user is null ? NotFound() : Ok(UserDto.From(user));
    }
}
