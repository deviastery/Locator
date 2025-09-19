using FluentValidation;
using Locator.Contracts.Ratings;
using Locator.Contracts.Vacancies;

namespace Locator.Application.Ratings;

public class CreateVacancyRatingDtoValidator : AbstractValidator<CreateVacancyRatingDto>
{
    public CreateVacancyRatingDtoValidator()
    {
        RuleFor(x => x.VacancyId)
            .NotEmpty().WithMessage("Id вакансии не может быть пустым").WithErrorCode("400");
        RuleFor(x => x.AverageMark)
            .NotEmpty().WithMessage("Поле оценки не может быть пустым").WithErrorCode("400")
            .GreaterThanOrEqualTo(0.0).WithMessage("Оценка должна быть в пределах от 0.0 до 5.0").WithErrorCode("400")
            .LessThanOrEqualTo(5.0).WithMessage("Оценка должна быть в пределах от 0.0 до 5.0").WithErrorCode("400");
    }
}