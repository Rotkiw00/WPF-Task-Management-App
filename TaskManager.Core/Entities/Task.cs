using TaskManager.Core.Enums;

namespace TaskManager.Core.Entities;

public class Task
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Status Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? DueDateTime { get; set; }
    public Person AssignedToId { get; set; }
}
