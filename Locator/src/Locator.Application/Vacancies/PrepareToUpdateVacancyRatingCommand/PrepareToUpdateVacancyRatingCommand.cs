using Locator.Application.Abstractions;

namespace Locator.Application.Vacancies.PrepareToUpdateVacancyRatingCommand;

public record PrepareToUpdateVacancyRatingCommand(string vacancyId) : ICommand;