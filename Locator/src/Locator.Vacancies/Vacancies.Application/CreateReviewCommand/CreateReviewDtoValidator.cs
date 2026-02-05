using FluentValidation;
using Vacancies.Contracts.Dto;

namespace Vacancies.Application.CreateReviewCommand;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.Mark).NotNull().WithMessage("The mark field cannot be empty").WithErrorCode("400")
            .GreaterThanOrEqualTo(0.0).WithMessage("The rating must be between 0.0 and 5.0").WithErrorCode("400")
            .LessThanOrEqualTo(5.0).WithMessage("The rating must be between 0.0 and 5.0").WithErrorCode("400");
        RuleFor(x => x.Comment)
            .MaximumLength(250).WithMessage("The comment cannot exceed 250 characters in length").WithErrorCode("400");
    }
}