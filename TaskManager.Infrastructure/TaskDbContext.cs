using Microsoft.EntityFrameworkCore;

namespace TaskManager.Infrastructure;

public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<Core.Entities.WorkTask> Tasks { get; set; }
    public DbSet<Core.Entities.Person>  People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
