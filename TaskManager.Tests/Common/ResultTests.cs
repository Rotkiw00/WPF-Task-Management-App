using TaskManager.Core.Common;

namespace TaskManager.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_CreatesSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.Equal("Operation completed successfully", result.Message);
    }

    [Fact]
    public void Success_WithMessage_CreatesSuccessfulResultWithMessage()
    {
        // Arrange
        var message = "Operation completed successfully";

        // Act
        var result = Result.Success(message);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.Equal(message, result.Message);
    }

    [Fact]
    public void Failure_CreatesFailedResult()
    {
        // Arrange
        var errorMessage = "Operation failed";

        // Act
        var result = Result.Failure(errorMessage, errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Contains(errorMessage, result.Errors);
        Assert.Equal(errorMessage, result.Message);
    }

    [Fact]
    public void Failure_WithMultipleErrors_CreatesFailedResultWithErrors()
    {
        // Arrange
        var message = "Multiple errors occurred";
        var errors = new List<string> { "Error 1", "Error 2", "Error 3" };

        // Act
        var result = Result.Failure(message, errors);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(3, result.Errors.Count);
        Assert.Equal(errors, result.Errors);
        Assert.Equal(message, result.Message);
    }
}

public class GenericResultTests
{
    [Fact]
    public void Success_WithData_CreatesSuccessfulResult()
    {
        // Arrange
        var data = "Test Data";

        // Act
        var result = Result<string>.Success(data);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.Equal(data, result.Data);
        Assert.Equal("Operation completed successfully", result.Message);
    }

    [Fact]
    public void Success_WithDataAndMessage_CreatesSuccessfulResultWithMessage()
    {
        // Arrange
        var data = 42;
        var message = "Number retrieved successfully";

        // Act
        var result = Result<int>.Success(data, message);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.Equal(data, result.Data);
        Assert.Equal(message, result.Message);
    }

    [Fact]
    public void Failure_CreatesFailedResultWithoutData()
    {
        // Arrange
        var errorMessage = "Failed to retrieve data";

        // Act
        var result = Result<string>.Failure(errorMessage, new List<string> { errorMessage });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Contains(errorMessage, result.Errors);
        Assert.Null(result.Data);
        Assert.Equal(errorMessage, result.Message);
    }

    [Fact]
    public void Failure_WithMultipleErrors_CreatesFailedResultWithErrors()
    {
        // Arrange
        var message = "Validation failed";
        var errors = new List<string> { "Validation error 1", "Validation error 2" };

        // Act
        var result = Result<object>.Failure(message, errors);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal(errors, result.Errors);
        Assert.Null(result.Data);
        Assert.Equal(message, result.Message);
    }

    [Fact]
    public void Success_WithComplexObject_WorksCorrectly()
    {
        // Arrange
        var person = new { Name = "John Doe", Age = 30 };

        // Act
        var result = Result<object>.Success(person);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(person, result.Data);
    }

    [Fact]
    public void Success_WithNull_CreatesSuccessfulResultWithNullData()
    {
        // Act
        var result = Result<string?>.Success(null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failure_WithEmptyErrorList_StillCreatesFailedResult()
    {
        // Arrange
        var message = "Operation failed";
        var errors = new List<string>();

        // Act
        var result = Result<int>.Failure(message, errors);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.Equal(0, result.Data);
        Assert.Equal(message, result.Message);
    }
}
