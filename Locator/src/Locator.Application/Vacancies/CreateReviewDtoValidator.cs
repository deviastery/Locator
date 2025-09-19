using FluentValidation;
using Locator.Contracts.Vacancies;

namespace Locator.Application.Vacancies;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.Mark).NotEmpty().WithMessage("Поле оценки не может быть пустым").WithErrorCode("400")
            .GreaterThanOrEqualTo(0.0).WithMessage("Оценка должна быть в пределах от 0.0 до 5.0").WithErrorCode("400")
            .LessThanOrEqualTo(5.0).WithMessage("Оценка должна быть в пределах от 0.0 до 5.0").WithErrorCode("400");
        RuleFor(x => x.Comment)
            .MaximumLength(250).WithMessage("Комментарий не может превышать длину 250 символов").WithErrorCode("400");
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Поле имени не может быть пустым").WithErrorCode("400")
            .MaximumLength(15).WithMessage("Имя не может превышать длину 15 символов").WithErrorCode("400");
    }
}