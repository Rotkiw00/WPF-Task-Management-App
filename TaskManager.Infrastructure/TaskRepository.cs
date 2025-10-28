using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infrastructure;

public class TaskRepository(TaskDbContext context) : ITaskRepository
{
    private readonly TaskDbContext _context = context;

    public async Task<List<WorkTask>> GetAllTasksAsync()
    {
        return await _context.Tasks
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<WorkTask?> GetTaskByIdAsync(Guid id)
    {
        return await _context.Tasks
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<WorkTask> AddTaskAsync(WorkTask task)
    {
        task.Id = Guid.NewGuid();
        task.CreatedDateTime = DateTime.Now;

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateTaskAsync(WorkTask task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(Guid id)
    {
        var task = await GetTaskByIdAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<WorkTask>> GetTasksByStatusAsync(Core.Enums.Status status)
    {
        return await _context.Tasks
            .Include(t => t.AssignedTo)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<WorkTask>> GetTasksByPriorityAsync(Core.Enums.Priority priority)
    {
        return await _context.Tasks
            .Include(t => t.AssignedTo)
            .Where(t => t.Priority == priority)
            .OrderByDescending(t => t.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<WorkTask>> GetTasksByPersonAsync(Guid personId)
    {
        return await _context.Tasks
            .Include(t => t.AssignedTo)
            .Where(t => t.AssignedTo != null && t.AssignedTo.Id == personId)
            .OrderByDescending(t => t.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<WorkTask>> SearchTasksAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllTasksAsync();

        var lowerSearch = searchTerm.ToLower();
        return await _context.Tasks
            .Include(t => t.AssignedTo)
            .Where(t => t.Title.Contains(lowerSearch, StringComparison.CurrentCultureIgnoreCase) ||
                            t.Description.Contains(lowerSearch, StringComparison.CurrentCultureIgnoreCase))
            .OrderByDescending(t => t.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<Person>> GetAllPeopleAsync()
    {
        return await _context.People
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
