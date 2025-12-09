using Shared.Abstractions;
using Vacancies.Contracts.Dto;

namespace Vacancies.Application.CreateRequestVacancyRatingCommand;

public record CreateRequestVacancyRatingCommand(CreateRequestVacancyRatingDto Dto) : ICommand;