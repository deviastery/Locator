using Locator.Application.Abstractions;
using Locator.Contracts.Ratings;

namespace Locator.Application.Ratings.UpdateVacancyRating;

public record UpdateVacancyRatingCommand(UpdateVacancyRatingDto vacancyRatingDto) : ICommand;
