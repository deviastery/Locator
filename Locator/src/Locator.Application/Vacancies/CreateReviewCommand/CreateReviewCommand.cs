using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;
using Locator.Contracts.Vacancies.Dtos;

namespace Locator.Application.Vacancies.CreateReviewCommand;

public record CreateReviewCommand(long VacancyId, Guid UserId, CreateReviewDto ReviewDto) : ICommand;