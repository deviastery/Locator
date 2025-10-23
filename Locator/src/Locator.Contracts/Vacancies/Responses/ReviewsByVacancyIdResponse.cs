using Locator.Contracts.Vacancies.Dtos;

namespace Locator.Contracts.Vacancies.Responses;

public record ReviewsByVacancyIdResponse(IEnumerable<ReviewDto> Reviews);