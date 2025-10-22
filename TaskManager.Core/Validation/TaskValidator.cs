using FluentValidation;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Validation
{
    public class TaskValidator : AbstractValidator<Entities.Task>
    {
        public TaskValidator()
        {
            RuleFor(t => t.Title)
                .Must(title => !string.IsNullOrWhiteSpace(title))
                .WithMessage("Title cannot be empty or whitespace.");

            RuleFor(t => t.DueDate)
                .GreaterThan(t => t.CreatedDateTime)
                .When(t => t.DueDate.HasValue)
                .WithMessage("Due date must be after the creation date.");

            RuleFor(t => t.EstimatedHours)
                .NotNull()
                .When(t => t.Status == Status.InProgress || t.Status == Status.UnderReview)
                .WithMessage("Estimated hours must be provided for tasks in progress or under review.");

            RuleFor(t => t.AssignedTo)
                .NotNull()
                .When(t => t.Status is Status.Assigned or Status.InProgress or Status.UnderReview)
                .WithMessage("Task must have an assignee in the current status.");

            RuleForEach(t => t.Tags)
                .Must(tag => !string.IsNullOrWhiteSpace(tag))
                .WithMessage("Tags cannot contain empty values.");

            RuleFor(t => t.EstimatedHours)
                .NotNull()
                .When(t => t.Status == Status.Completed)
                .WithMessage("Completed tasks must have estimated hours defined.");
        }
    }
}
