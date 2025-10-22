using TaskManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.Entities;

public class Task
{
    [Key]
    public Guid Id { get; set; }
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public Status Status { get; set; } = Status.Draft;
    [Required]
    public Priority Priority { get; set; } = Priority.Medium;

    public DateTime CreatedDateTime { get; set; }
    public DateTime? DueDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Estimated hours must be positive")]
    public int? EstimatedHours { get; set; }

    public Person? AssignedTo { get; set; }
    public List<string> Tags { get; set; } = [];
}
