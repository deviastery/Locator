using Locator.Domain.Ratings;
using Locator.Domain.Vacancies;

namespace Locator.Application.Vacancies;

public interface IVacanciesReadDbContext
{
    IQueryable<Vacancy> ReadVacancies { get; }
    IQueryable<Review> ReadReviews { get; }
}