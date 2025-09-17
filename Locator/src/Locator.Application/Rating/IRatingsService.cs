namespace Locator.Application.Rating;

public interface IRatingsService
{
    Task<Guid> CreateVacancyRating(
        Guid vacancyId,
        double averageMark,
        CancellationToken cancellationToken);
}