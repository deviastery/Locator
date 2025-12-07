using Shared.Abstractions;

namespace Vacancies.Application.PrepareToUpdateVacancyRatingCommand;

public record PrepareToUpdateVacancyRatingCommand(long VacancyId) : ICommand;