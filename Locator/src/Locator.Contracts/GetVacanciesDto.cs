namespace Locator.Contracts;

public record GetVacanciesDto(string Search, Guid UserId, bool IsResponsed, int PageSize, int Limit);