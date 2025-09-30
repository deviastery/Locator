using Locator.Domain.Ratings;

namespace Locator.Application.Ratings;

public interface IRatingsReadDbContext
{
    IQueryable<VacancyRating> ReadVacancyRatings { get; }
}