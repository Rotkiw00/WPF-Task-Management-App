using TaskManager.Core.Entities;
using TaskManager.Core.Enums;

namespace TaskManager.Infrastructure.Data;

public static class DataSeeder
{
    public static void SeedData(TaskDbContext context)
    {
        if (context.Tasks.Any() || context.People.Any())
            return; 

        var people = new List<Person>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Email = "john.doe@example.com"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Jane Smith",
                Email = "jane.smith@example.com"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Bob Johnson",
                Email = "bob.johnson@example.com"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Alice Williams",
                Email = "alice.williams@example.com"
            }
        };

        context.People.AddRange(people);
        context.SaveChanges();

        var tasks = new List<WorkTask>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Setup development environment",
                Description = "Install Visual Studio, .NET 8, and required tools",
                Status = Status.Completed,
                Priority = Priority.High,
                CreatedDateTime = DateTime.Now.AddDays(-10),
                DueDate = DateTime.Now.AddDays(-8),
                EstimatedHours = 4,
                AssignedTo = people[0],
                Tags = new List<string> { "setup", "environment", "dev" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Design database schema",
                Description = "Create Entity Relationship Diagram for the application",
                Status = Status.Completed,
                Priority = Priority.High,
                CreatedDateTime = DateTime.Now.AddDays(-9),
                DueDate = DateTime.Now.AddDays(-7),
                EstimatedHours = 6,
                AssignedTo = people[1],
                Tags = new List<string> { "database", "design" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Implement user authentication",
                Description = "Add login and registration functionality with JWT tokens",
                Status = Status.InProgress,
                Priority = Priority.Critical,
                CreatedDateTime = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(2),
                EstimatedHours = 8,
                AssignedTo = people[0],
                Tags = new List<string> { "security", "authentication", "backend" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Create WPF main window UI",
                Description = "Design and implement the main application window with MVVM pattern",
                Status = Status.InProgress,
                Priority = Priority.High,
                CreatedDateTime = DateTime.Now.AddDays(-4),
                DueDate = DateTime.Now.AddDays(3),
                EstimatedHours = 10,
                AssignedTo = people[2],
                Tags = new List<string> { "ui", "wpf", "frontend" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Write unit tests for TaskService",
                Description = "Implement comprehensive unit tests covering all CRUD operations",
                Status = Status.Assigned,
                Priority = Priority.Medium,
                CreatedDateTime = DateTime.Now.AddDays(-3),
                DueDate = DateTime.Now.AddDays(5),
                EstimatedHours = 6,
                AssignedTo = people[3],
                Tags = new List<string> { "testing", "unit-tests" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Optimize database queries",
                Description = "Review and optimize slow queries, add appropriate indexes",
                Status = Status.Draft,
                Priority = Priority.Low,
                CreatedDateTime = DateTime.Now.AddDays(-2),
                DueDate = DateTime.Now.AddDays(7),
                EstimatedHours = null,
                AssignedTo = null,
                Tags = new List<string> { "performance", "database", "optimization" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Deploy to production",
                Description = "Setup CI/CD pipeline and deploy the application to production environment",
                Status = Status.Draft,
                Priority = Priority.Medium,
                CreatedDateTime = DateTime.Now.AddDays(-1),
                DueDate = DateTime.Now.AddDays(10),
                EstimatedHours = null,
                AssignedTo = null,
                Tags = new List<string> { "deployment", "devops", "production" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Fix critical bug in payment module",
                Description = "Users reporting failed transactions - investigate and fix ASAP",
                Status = Status.UnderReview,
                Priority = Priority.Critical,
                CreatedDateTime = DateTime.Now.AddDays(-1),
                DueDate = DateTime.Now.AddDays(1),
                EstimatedHours = 3,
                AssignedTo = people[1],
                Tags = new List<string> { "bug", "critical", "payment" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Update documentation",
                Description = "Update API documentation and user manual for new features",
                Status = Status.Assigned,
                Priority = Priority.Low,
                CreatedDateTime = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                EstimatedHours = 5,
                AssignedTo = people[2],
                Tags = new List<string> { "documentation", "manual" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Research new UI framework",
                Description = "Evaluate alternatives to current WPF implementation",
                Status = Status.Rejected    ,
                Priority = Priority.Low,
                CreatedDateTime = DateTime.Now.AddDays(-15),
                DueDate = DateTime.Now.AddDays(-5),
                EstimatedHours = 8,
                AssignedTo = people[3],
                Tags = new List<string> { "research", "ui", "cancelled" }
            }
        };

        context.Tasks.AddRange(tasks);
        context.SaveChanges();
    }
}
