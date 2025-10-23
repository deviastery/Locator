using Locator.Application.Abstractions;
using Locator.Contracts.Ratings;
using Locator.Contracts.Ratings.Dtos;

namespace Locator.Application.Ratings.UpdateVacancyRatingCommand;

public record UpdateVacancyRatingCommand(UpdateVacancyRatingDto vacancyRatingDto) : ICommand;
