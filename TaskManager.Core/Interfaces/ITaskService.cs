using TaskManager.Core.Common;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Interfaces;

public interface ITaskService
{
    Task<Result<List<Entities.WorkTask>>> GetAllTasksAsync();
    Task<Result<Entities.WorkTask>> GetTaskByIdAsync(Guid id);
    Task<Result<Entities.WorkTask>> CreateTaskAsync(Entities.WorkTask task);
    Task<Result> UpdateTaskAsync(Entities.WorkTask task);
    Task<Result> DeleteTaskAsync(Guid id);

    Task<Result<List<Entities.WorkTask>>> FilterTasksAsync(
        Status? status = null,
        Priority? priority = null,
        Guid? assignedToId = null);

    Task<Result<List<Entities.WorkTask>>> SearchTasksAsync(string searchTerm);

    Task<Result<List<Entities.Person>>> GetAllPeopleAsync();
}