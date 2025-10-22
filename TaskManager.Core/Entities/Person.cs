using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.Entities;

public class Person
{
    [Key]
    public Guid Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    public ICollection<Task> Tasks { get; set; } = [];
}
