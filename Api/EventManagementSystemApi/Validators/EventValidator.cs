using EventManagementSystemApi.Models.DTOs;
using FluentValidation;

namespace EventManagementSystemApi.Validators
{
    public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
    {
        public CreateEventDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(50).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description is too long.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(50).WithMessage("Location cannot exceed 200 characters.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.")
                .LessThanOrEqualTo(10000).WithMessage("Capacity is unrealistically large.");

            RuleFor(x => x.DateTime)
                .Must(BeInFuture).WithMessage("Event date must be in the future.");

            RuleFor(x => x.Visibility)
                .NotEmpty().WithMessage("Visibility is required.")
                .Must(v => v == "true" || v == "false")
                .WithMessage("Visibility must be 'true' or 'false'.");
        }

        private bool BeInFuture(DateTime date)
        {
            return date > DateTime.UtcNow;
        }
    }
}
