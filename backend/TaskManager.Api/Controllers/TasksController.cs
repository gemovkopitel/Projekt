using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Dtos;
using TaskManager.Api.Models;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(AppDbContext db) : ControllerBase
{
    // GET /api/tasks?status=new
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAll([FromQuery] string? status)
    {
        if (status is not null && !TaskStatuses.IsValid(status))
        {
            ModelState.AddModelError(nameof(status), "Niepoprawny status.");
            return ValidationProblem(ModelState);
        }

        var query = db.Tasks.AsNoTracking();
        if (status is not null)
        {
            query = query.Where(t => t.Status == status);
        }

        var tasks = await query.OrderBy(t => t.Id).ToListAsync();
        return Ok(tasks.Select(TaskDto.From));
    }

    // GET /api/tasks/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetById(int id)
    {
        var task = await db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        return task is null ? NotFound() : Ok(TaskDto.From(task));
    }

    // POST /api/tasks
    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(CreateTaskRequest request)
    {
        var userId = request.UserId!.Value;
        if (!await db.Users.AnyAsync(u => u.Id == userId))
        {
            ModelState.AddModelError(nameof(request.UserId), "Użytkownik nie istnieje.");
            return ValidationProblem(ModelState);
        }

        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = NormalizeDescription(request.Description),
            Status = request.Status ?? TaskStatuses.New,
            UserId = userId,
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, TaskDto.From(task));
    }

    // PUT /api/tasks/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskDto>> Update(int id, UpdateTaskRequest request)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        task.Title = request.Title.Trim();
        task.Description = NormalizeDescription(request.Description);
        task.Status = request.Status;
        await db.SaveChangesAsync();

        return Ok(TaskDto.From(task));
    }

    // PATCH /api/tasks/5/status – przenoszenie karty między kolumnami
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TaskDto>> UpdateStatus(int id, UpdateStatusRequest request)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        task.Status = request.Status;
        await db.SaveChangesAsync();

        return Ok(TaskDto.From(task));
    }

    // DELETE /api/tasks/5 – komentarze usuwa baza (ON DELETE CASCADE)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await db.Tasks.Where(t => t.Id == id).ExecuteDeleteAsync();
        return deleted == 0 ? NotFound() : NoContent();
    }

    // GET /api/tasks/5/comments
    [HttpGet("{id:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(int id)
    {
        if (!await db.Tasks.AnyAsync(t => t.Id == id))
        {
            return NotFound();
        }

        var comments = await db.Comments.AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.TaskId == id)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
        return Ok(comments.Select(CommentDto.From));
    }

    // POST /api/tasks/5/comments
    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<CommentDto>> AddComment(int id, CreateCommentRequest request)
    {
        if (!await db.Tasks.AnyAsync(t => t.Id == id))
        {
            return NotFound();
        }

        var user = await db.Users.FindAsync(request.UserId!.Value);
        if (user is null)
        {
            ModelState.AddModelError(nameof(request.UserId), "Użytkownik nie istnieje.");
            return ValidationProblem(ModelState);
        }

        var comment = new Comment { Content = request.Content.Trim(), TaskId = id, UserId = user.Id, User = user };
        db.Comments.Add(comment);
        await db.SaveChangesAsync();

        return Created($"/api/comments/{comment.Id}", CommentDto.From(comment));
    }

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}
