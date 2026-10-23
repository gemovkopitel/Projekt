using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Models;

namespace TaskManager.Api.Dtos;

public record CommentDto(int Id, string Content, DateTime CreatedAt, int TaskId, int UserId, string UserLogin)
{
    public static CommentDto From(Comment comment) =>
        new(comment.Id, comment.Content, comment.CreatedAt, comment.TaskId, comment.UserId, comment.User.Login);
}

public record CreateCommentRequest(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Treść komentarza jest wymagana.")]
    string Content,

    [Required(ErrorMessage = "Użytkownik jest wymagany.")]
    int? UserId);
