namespace TaskManager.Api.Models;

// Wartości zgodne z frontendem (frontend/js/app.js -> STATUSES).
public static class TaskStatuses
{
    public const string New = "new";
    public const string InProgress = "in_progress";
    public const string Done = "done";

    public static readonly string[] All = [New, InProgress, Done];

    public static bool IsValid(string? status) => status is not null && All.Contains(status);
}
