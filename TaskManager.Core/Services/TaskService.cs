using FluentValidation;
using TaskManager.Core.Common;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Validation;

namespace TaskManager.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly TaskValidator _validator;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
        _validator = new TaskValidator();
    }

    public async Task<Result<List<WorkTask>>> GetAllTasksAsync()
    {
        try
        {
            var tasks = await _repository.GetAllTasksAsync();
            return Result<List<WorkTask>>.Success(tasks, $"Retrieved {tasks.Count} tasks");
        }
        catch (Exception ex)
        {
            return Result<List<WorkTask>>.Failure(
                "Failed to retrieve tasks",
                new List<string> { ex.Message });
        }
    }

    public async Task<Result<WorkTask>> GetTaskByIdAsync(Guid id)
    {
        try
        {
            var task = await _repository.GetTaskByIdAsync(id);

            if (task == null)
                return Result<WorkTask>.Failure("Task not found", new List<string> { $"No task with ID {id}" });

            return Result<WorkTask>.Success(task, "Task retrieved successfully");
        }
        catch (Exception ex)
        {
            return Result<WorkTask>.Failure(
                "Failed to retrieve task",
                new List<string> { ex.Message });
        }
    }

    public async Task<Result<WorkTask>> CreateTaskAsync(WorkTask task)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(task);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<WorkTask>.Failure("Validation failed", errors);
            }

            var createdTask = await _repository.AddTaskAsync(task);
            return Result<WorkTask>.Success(createdTask, "Task created successfully");
        }
        catch (Exception ex)
        {
            return Result<WorkTask>.Failure(
                "Failed to create task",
                new List<string> { ex.Message });
        }
    }

    public async Task<Result> UpdateTaskAsync(WorkTask task)
    {
        try
        {
            var existingTask = await _repository.GetTaskByIdAsync(task.Id);
            if (existingTask == null)
                return Result.Failure("Task not found", $"No task with ID {task.Id}");

            var validationResult = await _validator.ValidateAsync(task);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result.Failure("Validation failed", errors);
            }

            await _repository.UpdateTaskAsync(task);
            return Result.Success("Task updated successfully");
        }
        catch (Exception ex)
        {
            return Result.Failure(
                "Failed to update task",
                new List<string> { ex.Message });
        }
    }

    public async Task<Result> DeleteTaskAsync(Guid id)
    {
        try
        {
            var existingTask = await _repository.GetTaskByIdAsync(id);
            if (existingTask == null)
                return Result.Failure("Task not found", $"No task with ID {id}");

            await _repository.DeleteTaskAsync(id);
            return Result.Success("Task deleted successfully");
        }
        catch (Exception ex)
        {
            return Result.Failure(
                "Failed to delete task",
                new List<string> { ex.Message });
        }
    }

    public async Task<Result<List<WorkTask>>> FilterTasksAsync(
        Status? status = null,
        Priority? priority = null,
        Guid? assignedToId = null)
    {
        try
        {
            List<WorkTask> tasks;

            if (status.HasValue && priority.HasValue && assignedToId.HasValue)
            {
                tasks = await _repository.GetAllTasksAsync();
                tasks = tasks.Where(t =>
                    t.Status == status.Value &&
                    t.Priority == priority.Value &&
                    t.AssignedTo != null && t.AssignedTo.Id == assignedToId.Value
                ).ToList();
            }
            else if (status.HasValue)
            {
                tasks = await _repository.GetTasksByStatusAsync(status.Value);
            }
            else if (priority.HasValue)
            {
                tasks = await _repository.GetTasksByPriorityAsync(priority.Value);
            }
            else if (assignedToId.HasValue)
            {
                tasks = await _repository.GetTasksByPersonAsync(assignedToId.Value);
            }
            else
            {
                tasks = await _repository.GetAllTasksAsync();
            }

            return Result<List<WorkTask>>.Success(tasks, $"Found {tasks.Count} tasks");
        }
        catch (Exception ex)
        {
            return Result<List<WorkTask>>.Failure(
                "Failed to filter tasks",
                new List<string> { ex.Message });
        }
    }

    public async Task<Result<List<WorkTask>>> SearchTasksAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllTasksAsync();

            var tasks = await _repository.SearchTasksAsync(searchTerm);
            return Result<List<WorkTask>>.Success(tasks, $"Found {tasks.Count} tasks matching '{searchTerm}'");
        }
        catch (Exception ex)
        {
            return Result<List<WorkTask>>.Failure(
                "Failed to search tasks",
                new List<string> { ex.Message });
        }
    }

    public async Task<Result<List<Person>>> GetAllPeopleAsync()
    {
        try
        {
            var people = await _repository.GetAllPeopleAsync();
            return Result<List<Person>>.Success(people, $"Retrieved {people.Count} people");
        }
        catch (Exception ex)
        {
            return Result<List<Person>>.Failure(
                "Failed to retrieve people",
                new List<string> { ex.Message });
        }
    }
}