using FluentValidation;
using SmartTaskManagement.Application.DTOs.Projects;

namespace SmartTaskManagement.Application.Validators
{
    public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .LessThanOrEqualTo(DateTime.UtcNow.AddYears(1))
                .WithMessage("Start date cannot be more than 1 year in the future");

            RuleFor(x => x)
                .Must(x => !x.EndDate.HasValue || x.EndDate > x.StartDate)
                .WithMessage("End date must be after start date");
        }
    }

    public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x)
                .Must(x => !x.EndDate.HasValue || x.EndDate > x.StartDate)
                .WithMessage("End date must be after start date");
        }
    }

    public class ProjectFilterDtoValidator : AbstractValidator<ProjectFilterDto>
    {
        public ProjectFilterDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters");

            RuleFor(x => x.SortBy)
                .Must(x => string.IsNullOrEmpty(x) ||
                    new[] { "name", "startdate", "enddate", "isactive", "taskcount", "createdat" }
                        .Contains(x.ToLower()))
                .WithMessage("Invalid sort field");
        }
    }
}