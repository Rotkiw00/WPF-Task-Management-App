using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Validation;

namespace TaskManager.Tests.Validation;

public class TaskValidatorTests
{
    private readonly TaskValidator _validator;

    public TaskValidatorTests()
    {
        _validator = new TaskValidator();
    }

    [Fact]
    public void Validate_ValidTask_ReturnsSuccess()
    {
        // Arrange
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

        // Act
        var result = _validator.Validate(task);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyTitle_ReturnsFail()
    {
        // Arrange
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "",
            Status = Status.Draft,
            Priority = Priority.Medium,
            CreatedDateTime = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var result = _validator.Validate(task);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_DueDateBeforeCreatedDate_ReturnsFail()
    {
        // Arrange
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Status = Status.Draft,
            Priority = Priority.Medium,
            CreatedDateTime = DateTime.Now,
            DueDate = DateTime.Now.AddDays(-1) // DueDate in the past
        };

        // Act
        var result = _validator.Validate(task);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DueDate");
    }

    [Fact]
    public void Validate_InProgressWithoutEstimatedHours_ReturnsFail()
    {
        // Arrange
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Status = Status.InProgress,
            Priority = Priority.Medium,
            CreatedDateTime = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7),
            EstimatedHours = null, // EstimatedHours jest null (not zero)
            AssignedTo = new Person { Id = Guid.NewGuid(), Name = "John Doe" } // Must have assignee for InProgress
        };

        // Act
        var result = _validator.Validate(task);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EstimatedHours");
    }

    [Fact]
    public void Validate_AssignedStatusWithoutAssignedPerson_ReturnsFail()
    {
        // Arrange
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Status = Status.Assigned,
            Priority = Priority.Medium,
            CreatedDateTime = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7),
            EstimatedHours = 5,
            AssignedTo = null // Missing assigned person
        };

        // Act
        var result = _validator.Validate(task);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "AssignedTo");
    }

    [Fact]
    public void Validate_InProgressWithAssignedPerson_ReturnsSuccess()
    {
        // Arrange
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Status = Status.InProgress,
            Priority = Priority.High,
            CreatedDateTime = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7),
            EstimatedHours = 10,
            AssignedTo = new Person { Id = Guid.NewGuid(), Name = "John Doe" }
        };

        // Act
        var result = _validator.Validate(task);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_CompletedStatusWithoutEstimatedHours_PassesValidation()
    {
        // Arrange - Completed nie wymaga EstimatedHours w walidatorze
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Status = Status.Completed,
            Priority = Priority.Medium,
            CreatedDateTime = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7),
            EstimatedHours = 0 // Completed nie wymaga EstimatedHours według walidatora
        };

        // Act
        var result = _validator.Validate(task);

        // Assert
        Assert.True(result.IsValid); // Powinno przejść walidację
    }

    [Fact]
    public void Validate_UnderReviewWithAssignedPerson_ReturnsSuccess()
    {
        // Arrange
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Status = Status.UnderReview,
            Priority = Priority.Critical,
            CreatedDateTime = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7),
            EstimatedHours = 15,
            AssignedTo = new Person { Id = Guid.NewGuid(), Name = "Jane Smith" }
        };

        // Act
        var result = _validator.Validate(task);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_DraftStatusWithoutAssignedPerson_ReturnsSuccess()
    {
        // Arrange - Draft nie wymaga przypisanej osoby
        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Status = Status.Draft,
            Priority = Priority.Low,
            CreatedDateTime = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var result = _validator.Validate(task);

        // Assert
        Assert.True(result.IsValid);
    }
}
