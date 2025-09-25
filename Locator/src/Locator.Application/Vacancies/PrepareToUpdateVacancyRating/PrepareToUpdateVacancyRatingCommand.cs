using Locator.Application.Abstractions;

namespace Locator.Application.Vacancies.PrepareToUpdateVacancyRating;

public record PrepareToUpdateVacancyRatingCommand(Guid vacancyId) : ICommand;