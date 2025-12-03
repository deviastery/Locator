using Vacancies.Domain;

namespace Vacancies.Application;

public interface IVacanciesReadDbContext
{
    IQueryable<Review> ReadReviews { get; }
}