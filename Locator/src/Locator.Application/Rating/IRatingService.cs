namespace Locator.Application.Rating;

public interface IRatingService
{
    Task<Guid> CreateVacancyRating(
        Guid vacancyId,
        double averageMark,
        CancellationToken cancellationToken);
}