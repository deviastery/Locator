using Locator.Contracts.Vacancies.Dto;

namespace Locator.Contracts.Vacancies.Responses;

public record NegotiationsResponse(
    long Count, 
    IEnumerable<NegotiationDto> Negotiations, 
    int Page, 
    int Pages, 
    int PerPage);