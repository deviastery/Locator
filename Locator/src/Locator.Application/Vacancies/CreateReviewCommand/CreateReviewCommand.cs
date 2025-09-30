using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;

namespace Locator.Application.Vacancies.CreateReviewCommand;

public record CreateReviewCommand(Guid vacancyId, CreateReviewDto reviewDto) : ICommand;