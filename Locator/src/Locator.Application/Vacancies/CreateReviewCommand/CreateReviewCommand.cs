using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies.Dto;

namespace Locator.Application.Vacancies.CreateReviewCommand;

public record CreateReviewCommand(long VacancyId, Guid UserId, CreateReviewDto ReviewDto) : ICommand;