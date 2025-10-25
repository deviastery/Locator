using FluentValidation;
using Locator.Contracts.Vacancies.Dto;

namespace Locator.Application.Vacancies.CreateReviewCommand;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.NegotiationId).NotEmpty().WithMessage("The negotiation Id field cannot be empty").WithErrorCode("400")
            .GreaterThan(0).WithMessage("The negotiation Id must be positive").WithErrorCode("400")
            .LessThan(100000000000).WithMessage("Too big number").WithErrorCode("400");
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