using Shared.Abstractions;
using Vacancies.Contracts.Dto;

namespace Vacancies.Application.CreateReviewCommand;

public record CreateReviewCommand(long VacancyId, Guid UserId, CreateReviewDto ReviewDto) : ICommand;