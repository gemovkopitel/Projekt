using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data;

// Mapowanie na istniejący schemat bazy Task_manager (tabele users, tasks, comments).
// Schemat jest utrzymywany w bazie, więc API nie używa migracji EF.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.Login).HasColumnName("login").HasMaxLength(50);
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
            entity.Property(u => u.Role).HasColumnName("role").HasMaxLength(20).HasDefaultValue("user");
            entity.Property(u => u.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasColumnName("id");
            entity.Property(t => t.Title).HasColumnName("title").HasMaxLength(200);
            entity.Property(t => t.Description).HasColumnName("description");
            entity.Property(t => t.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue(TaskStatuses.New);
            entity.Property(t => t.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(t => t.UserId).HasColumnName("user_id");

            entity.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.Content).HasColumnName("content");
            entity.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(c => c.TaskId).HasColumnName("task_id");
            entity.Property(c => c.UserId).HasColumnName("user_id");

            entity.HasOne(c => c.Task).WithMany(t => t.Comments).HasForeignKey(c => c.TaskId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.NoAction);
        });
    }
}
