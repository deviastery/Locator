using Locator.Application.Vacancies;
using Locator.Domain.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Infrastructure.Postgresql.Vacancies;

public class VacanciesEfCoreRepository : IVacanciesRepository
{
    private readonly LocatorDbContext _locatorDbContext;

    public VacanciesEfCoreRepository(LocatorDbContext locatorDbContext)
    {
        _locatorDbContext = locatorDbContext;
    }

    public async Task<Guid> CreateReviewAsync(Review review, CancellationToken cancellationToken)
    {
        await _locatorDbContext.Reviews.AddAsync(review, cancellationToken);
        await _locatorDbContext.SaveChangesAsync(cancellationToken);
        return review.Id;
    }

    public async Task<List<Review>> GetReviewsByVacancyIdAsync(Guid vacancyId, CancellationToken cancellationToken)
    {
        var reviews = await _locatorDbContext.Reviews
            .Where(r => r.VacancyId == vacancyId)
            .ToListAsync(cancellationToken);
        return reviews;
    }

    public async Task<int> GetDaysAfterApplyingAsync(Guid vacancyId, Guid userId, CancellationToken cancellationToken)
    {
        // TODO: Соответствующий запрос на HH Api + бизнес логика
        return 5;
    }
}