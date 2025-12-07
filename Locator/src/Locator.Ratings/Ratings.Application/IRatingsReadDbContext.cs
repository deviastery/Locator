using Ratings.Domain;

namespace Ratings.Application;

public interface IRatingsReadDbContext
{
    IQueryable<VacancyRating> ReadVacancyRatings { get; }
}