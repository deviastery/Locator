using FluentValidation;
using Locator.Contracts.Vacancies;

namespace Locator.Application.Vacancies.CreateReview;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.Mark).NotEmpty().WithMessage("The mark field cannot be empty").WithErrorCode("400")
            .GreaterThanOrEqualTo(0.0).WithMessage("The rating must be between 0.0 and 5.0").WithErrorCode("400")
            .LessThanOrEqualTo(5.0).WithMessage("The rating must be between 0.0 and 5.0").WithErrorCode("400");
        RuleFor(x => x.Comment)
            .MaximumLength(250).WithMessage("The comment cannot exceed 250 characters in length").WithErrorCode("400");
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("The name field cannot be empty").WithErrorCode("400")
            .MaximumLength(15).WithMessage("The name cannot exceed 15 characters in length").WithErrorCode("400");
    }
}