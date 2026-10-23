using TaskManager.Api.Models;

namespace TaskManager.Api.Dtos;

// Bez password_hash – hasła nigdy nie wychodzą z API.
public record UserDto(int Id, string Login, string Email, string Role, DateTime CreatedAt)
{
    public static UserDto From(User user) =>
        new(user.Id, user.Login, user.Email, user.Role, user.CreatedAt);
}
