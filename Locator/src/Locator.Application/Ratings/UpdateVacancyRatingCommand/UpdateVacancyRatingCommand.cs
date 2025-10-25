using Locator.Application.Abstractions;
using Locator.Contracts.Ratings.Dto;

namespace Locator.Application.Ratings.UpdateVacancyRatingCommand;

public record UpdateVacancyRatingCommand(UpdateVacancyRatingDto VacancyRatingDto) : ICommand;
