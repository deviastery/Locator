using Ratings.Contracts.Dto;
using Shared.Abstractions;

namespace Ratings.Application.UpdateVacancyRatingCommand;

public record UpdateVacancyRatingCommand(UpdateVacancyRatingDto VacancyRatingDto) : ICommand;
