using FluentValidation;
using SmartTaskManagement.Application.DTOs.Tasks;


namespace SmartTaskManagement.Application.Validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required")
                .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required")
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");

            RuleFor(x => x.EstimatedHours)
                .GreaterThanOrEqualTo(0).WithMessage("Estimated hours must be greater than or equal to 0")
                .LessThanOrEqualTo(999).WithMessage("Estimated hours cannot exceed 999");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority value");
        }
    }

    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required")
                .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required");

            RuleFor(x => x.EstimatedHours)
                .GreaterThanOrEqualTo(0).WithMessage("Estimated hours must be greater than or equal to 0")
                .LessThanOrEqualTo(999).WithMessage("Estimated hours cannot exceed 999");

            RuleFor(x => x.ActualHours)
                .GreaterThanOrEqualTo(0).WithMessage("Actual hours must be greater than or equal to 0")
                .LessThanOrEqualTo(999).WithMessage("Actual hours cannot exceed 999");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority value");
        }
    }

    public class UpdateTaskStatusDtoValidator : AbstractValidator<UpdateTaskStatusDto>
    {
        public UpdateTaskStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters");
        }
    }

    public class TaskFilterDtoValidator : AbstractValidator<TaskFilterDto>
    {
        public TaskFilterDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters");

            RuleFor(x => x.SortBy)
                .Must(x => string.IsNullOrEmpty(x) ||
                    new[] { "title", "status", "priority", "duedate", "estimatedhours",
                        "actualhours", "projectname", "assignedto", "createdat" }
                        .Contains(x.ToLower()))
                .WithMessage("Invalid sort field");

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Invalid status value");

            RuleFor(x => x.Priority)
                .IsInEnum().When(x => x.Priority.HasValue)
                .WithMessage("Invalid priority value");
        }
    }
}