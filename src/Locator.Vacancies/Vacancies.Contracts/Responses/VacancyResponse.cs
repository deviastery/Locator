using Vacancies.Contracts.Dto;

namespace Vacancies.Contracts.Responses;

public record VacancyResponse(FullVacancyDto Vacancy, IEnumerable<ReviewDto>? Reviews);