using Locator.Application.Vacancies;
using Locator.Domain.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Infrastructure.Postgresql.Vacancies;

public class VacanciesEfCoreRepository : IVacanciesRepository
{
    private readonly DbContext _dbContext;

    public VacanciesEfCoreRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateReviewAsync(Review review, CancellationToken cancellationToken)
    {
        await _dbContext.Reviews.AddAsync(review, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return review.Id;
    }

    public async Task<List<Review>> GetReviewsByVacancyIdAsync(Guid vacancyId, CancellationToken cancellationToken)
    {
        var reviews = await _dbContext.Reviews
            .Where(r => r.VacancyId == vacancyId)
            .ToListAsync(cancellationToken);
        return reviews;
    }

    public async Task<int> GetDaysAfterApplyingAsync(Guid vacancyId, Guid UserId, CancellationToken cancellationToken)
    {
        // TODO: Соответствующий запрос на HH Api + бизнес логика
        return 5;
    }
}