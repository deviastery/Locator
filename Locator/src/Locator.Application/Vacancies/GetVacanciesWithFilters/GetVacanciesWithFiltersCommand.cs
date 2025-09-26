using Locator.Application.Abstractions;

namespace Locator.Application.Vacancies.GetVacanciesWithFilters;

public record GetVacanciesWithFiltersCommand(
    int PageNumber, 
    int PageSize, 
    string Search) : ICommand;