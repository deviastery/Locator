using Locator.Contracts.Vacancies.Dto;

namespace Locator.Contracts.Vacancies.Responses;

public record ReviewsByVacancyIdResponse(IEnumerable<ReviewDto> Reviews);