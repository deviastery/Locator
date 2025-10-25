using Locator.Domain.Vacancies;

namespace Locator.Application.Vacancies;

public interface IVacanciesReadDbContext
{
    IQueryable<Review> ReadReviews { get; }
}