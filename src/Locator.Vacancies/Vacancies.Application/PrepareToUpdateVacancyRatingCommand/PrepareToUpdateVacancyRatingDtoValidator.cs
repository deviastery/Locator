using FluentValidation;
using Vacancies.Contracts.Dto;

namespace Vacancies.Application.PrepareToUpdateVacancyRatingCommand;

public class PrepareToUpdateVacancyRatingDtoValidator : AbstractValidator<UpdateVacancyRatingDto>
{
    public PrepareToUpdateVacancyRatingDtoValidator()
    {
        RuleFor(x => x.VacancyId)
            .NotEmpty().WithMessage("The vacancy Id cannot be empty").WithErrorCode("500");
        RuleFor(x => x.AverageMark).NotEmpty().WithMessage("The mark field cannot be empty").WithErrorCode("400")
            .GreaterThanOrEqualTo(0.0).WithMessage("The rating must be between 0.0 and 5.0").WithErrorCode("400")
            .LessThanOrEqualTo(5.0).WithMessage("The rating must be between 0.0 and 5.0").WithErrorCode("400");
    }
}