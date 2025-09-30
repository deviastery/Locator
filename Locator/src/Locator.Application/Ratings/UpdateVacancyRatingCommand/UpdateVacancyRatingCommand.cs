using Locator.Application.Abstractions;
using Locator.Contracts.Ratings;

namespace Locator.Application.Ratings.UpdateVacancyRatingCommand;

public record UpdateVacancyRatingCommand(UpdateVacancyRatingDto vacancyRatingDto) : ICommand;
