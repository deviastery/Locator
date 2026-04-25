using Vacancies.Contracts.Dto;

namespace Vacancies.Contracts.Responses;

public record ReviewsByVacancyIdResponse(IEnumerable<ReviewDto> Reviews);