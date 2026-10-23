using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Models;

namespace TaskManager.Api.Dtos;

public record TaskDto(int Id, string Title, string? Description, string Status, DateTime CreatedAt, int UserId)
{
    public static TaskDto From(TaskItem task) =>
        new(task.Id, task.Title, task.Description, task.Status, task.CreatedAt, task.UserId);
}

public record CreateTaskRequest(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Tytuł jest wymagany.")]
    [MaxLength(200, ErrorMessage = "Tytuł może mieć maksymalnie 200 znaków.")]
    string Title,

    string? Description,

    [AllowedValues(TaskStatuses.New, TaskStatuses.InProgress, TaskStatuses.Done, null, ErrorMessage = "Niepoprawny status.")]
    string? Status,

    [Required(ErrorMessage = "Użytkownik jest wymagany.")]
    int? UserId);

public record UpdateTaskRequest(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Tytuł jest wymagany.")]
    [MaxLength(200, ErrorMessage = "Tytuł może mieć maksymalnie 200 znaków.")]
    string Title,

    string? Description,

    [Required(ErrorMessage = "Status jest wymagany.")]
    [AllowedValues(TaskStatuses.New, TaskStatuses.InProgress, TaskStatuses.Done, ErrorMessage = "Niepoprawny status.")]
    string Status);

public record UpdateStatusRequest(
    [Required(ErrorMessage = "Status jest wymagany.")]
    [AllowedValues(TaskStatuses.New, TaskStatuses.InProgress, TaskStatuses.Done, ErrorMessage = "Niepoprawny status.")]
    string Status);
