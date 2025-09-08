using FluentValidation;
using Locator.Contracts.Rating;
using Locator.Domain.Vacancies;

namespace Locator.Application.Rating;

public class CreateVacancyRatingDtoValidator : AbstractValidator<CreateVacancyRatingDto>
{
    public CreateVacancyRatingDtoValidator()
    {
        RuleForEach(x => x.Reviews).SetValidator(new ReviewValidator());
    }
}
public class ReviewValidator : AbstractValidator<Review>
{
    public ReviewValidator()
    {
        RuleFor(x => x.Mark).NotEmpty().WithMessage("Поле оценки не может быть пустым")
            .GreaterThanOrEqualTo(0.0).WithMessage("Оценка должна быть в пределах от 0.0 до 5.0")
            .LessThanOrEqualTo(5.0).WithMessage("Оценка должна быть в пределах от 0.0 до 5.0");
        RuleFor(x => x.Comment)
            .MaximumLength(250).WithMessage("Комментарий не может превышать длину 250 символов");
        RuleFor(x => x.UserId).NotEmpty();
    }
}