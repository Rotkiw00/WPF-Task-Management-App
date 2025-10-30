using TaskManager.Core.Entities;
using TaskManager.Core.Enums;

namespace TaskManager.Tests.Entities;

public class WorkTaskTests
{
    [Fact]
    public void WorkTask_CanBeCreated_WithRequiredProperties()
    {
        // Arrange & Act
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Description = "Test Description",
            Status = Status.Draft,
            Priority = Priority.Medium,
            CreatedDateTime = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7),
            EstimatedHours = 5
        };

        // Assert
        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal("Test Task", task.Title);
        Assert.Equal("Test Description", task.Description);
        Assert.Equal(Status.Draft, task.Status);
        Assert.Equal(Priority.Medium, task.Priority);
        Assert.True(task.CreatedDateTime <= DateTime.Now);
        Assert.NotNull(task.DueDate);
        Assert.Equal(5, task.EstimatedHours);
    }

    [Fact]
    public void WorkTask_CanHaveAssignedPerson()
    {
        // Arrange
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john@example.com"
        };

        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Assigned Task",
            Status = Status.Assigned,
            Priority = Priority.High,
            CreatedDateTime = DateTime.Now,
            AssignedTo = person
        };

        // Act & Assert
        Assert.NotNull(task.AssignedTo);
        Assert.Equal("John Doe", task.AssignedTo.Name);
        Assert.Equal(person.Id, task.AssignedTo.Id);
    }

    [Fact]
    public void WorkTask_CanHaveTags()
    {
        // Arrange
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Tagged Task",
            Status = Status.Draft,
            Priority = Priority.Low,
            CreatedDateTime = DateTime.Now,
            Tags = new List<string> { "backend", "urgent", "bug-fix" }
        };

        // Act & Assert
        Assert.NotNull(task.Tags);
        Assert.Equal(3, task.Tags.Count);
        Assert.Contains("backend", task.Tags);
        Assert.Contains("urgent", task.Tags);
        Assert.Contains("bug-fix", task.Tags);
    }

    [Fact]
    public void WorkTask_CanBeCreatedWithoutOptionalProperties()
    {
        // Arrange & Act
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Minimal Task",
            Status = Status.Draft,
            Priority = Priority.Low,
            CreatedDateTime = DateTime.Now
        };

        // Assert
        Assert.Equal(string.Empty, task.Description); // Description has default value
        Assert.Null(task.DueDate);
        Assert.Null(task.AssignedTo);
        Assert.NotNull(task.Tags); // Tags has default empty list
        Assert.Empty(task.Tags);
        Assert.Null(task.EstimatedHours); // EstimatedHours is nullable, default is null
    }

    [Fact]
    public void WorkTask_PropertiesCanBeUpdated()
    {
        // Arrange
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Original Title",
            Status = Status.Draft,
            Priority = Priority.Low,
            CreatedDateTime = DateTime.Now
        };

        // Act
        task.Title = "Updated Title";
        task.Status = Status.InProgress;
        task.Priority = Priority.Critical;
        task.EstimatedHours = 10;

        // Assert
        Assert.Equal("Updated Title", task.Title);
        Assert.Equal(Status.InProgress, task.Status);
        Assert.Equal(Priority.Critical, task.Priority);
        Assert.Equal(10, task.EstimatedHours);
    }
}

public class PersonTests
{
    [Fact]
    public void Person_CanBeCreated_WithRequiredProperties()
    {
        // Arrange & Act
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        // Assert
        Assert.NotEqual(Guid.Empty, person.Id);
        Assert.Equal("John Doe", person.Name);
        Assert.Equal("john.doe@example.com", person.Email);
    }

    [Fact]
    public void Person_CanHaveMultipleTasks()
    {
        // Arrange
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Jane Smith",
            Email = "jane@example.com"
        };

        var task1 = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Task 1",
            Status = Status.InProgress,
            Priority = Priority.High,
            CreatedDateTime = DateTime.Now,
            AssignedTo = person
        };

        var task2 = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Task 2",
            Status = Status.Assigned,
            Priority = Priority.Medium,
            CreatedDateTime = DateTime.Now,
            AssignedTo = person
        };

        person.Tasks = new List<WorkTask> { task1, task2 };

        // Act & Assert
        Assert.NotNull(person.Tasks);
        Assert.Equal(2, person.Tasks.Count);
        Assert.All(person.Tasks, t => Assert.Equal(person.Id, t.AssignedTo?.Id));
    }

    [Fact]
    public void Person_TasksCollection_CanBeEmpty()
    {
        // Arrange & Act
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Alice Williams",
            Email = "alice@example.com"
        };

        // Assert
        Assert.NotNull(person.Tasks);
        Assert.Empty(person.Tasks); // Tasks has default empty collection
    }
}
