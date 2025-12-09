using HeadHunter.Contracts.Dto;

namespace Vacancies.Contracts.Responses;

public record NegotiationsResponse(
    long Count, 
    IEnumerable<NegotiationDto> Negotiations, 
    int Page, 
    int Pages, 
    int PerPage);