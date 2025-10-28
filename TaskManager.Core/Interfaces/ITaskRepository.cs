namespace TaskManager.Core.Interfaces;

public interface ITaskRepository
{
    Task<List<Entities.WorkTask>> GetAllTasksAsync();
    Task<Entities.WorkTask?> GetTaskByIdAsync(Guid id);
    Task<Entities.WorkTask> AddTaskAsync(Entities.WorkTask task);
    Task UpdateTaskAsync(Entities.WorkTask task);
    Task DeleteTaskAsync(Guid id);

    Task<List<Entities.WorkTask>> GetTasksByStatusAsync(Enums.Status status);
    Task<List<Entities.WorkTask>> GetTasksByPriorityAsync(Enums.Priority priority);
    Task<List<Entities.WorkTask>> GetTasksByPersonAsync(Guid personId);
    Task<List<Entities.WorkTask>> SearchTasksAsync(string searchTerm);

    Task<List<Entities.Person>> GetAllPeopleAsync();
}
