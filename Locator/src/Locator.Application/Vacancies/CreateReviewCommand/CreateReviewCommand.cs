using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;

namespace Locator.Application.Vacancies.CreateReviewCommand;

public record CreateReviewCommand(long vacancyId, CreateReviewDto reviewDto) : ICommand;