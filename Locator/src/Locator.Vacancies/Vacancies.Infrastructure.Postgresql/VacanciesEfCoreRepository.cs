using Microsoft.EntityFrameworkCore;
using Vacancies.Application;
using Vacancies.Domain;

namespace Vacancies.Infrastructure.Postgresql;

public class VacanciesEfCoreRepository : IVacanciesRepository
{
    private readonly VacanciesDbContext _vacanciesDbContext;

    public VacanciesEfCoreRepository(VacanciesDbContext vacanciesDbContext)
    {
         _vacanciesDbContext = vacanciesDbContext;
    }

    public async Task<bool> HasUserReviewedVacancyAsync(
        Guid userId,
        long vacancyId,
        CancellationToken cancellationToken)
    {
        var review = await _vacanciesDbContext.Reviews
            .Where(r => r.VacancyId == vacancyId && r.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
        return review != null;
    }

    public async Task<Guid> CreateReviewAsync(Review review, CancellationToken cancellationToken)
    {
        await _vacanciesDbContext.Reviews.AddAsync(review, cancellationToken);
        await _vacanciesDbContext.SaveChangesAsync(cancellationToken);
        return review.Id;
    }

    public async Task<List<Review>> GetReviewsByVacancyIdAsync(long vacancyId, CancellationToken cancellationToken)
    {
        var reviews = await _vacanciesDbContext.Reviews
            .Where(r => r.VacancyId == vacancyId)
            .ToListAsync(cancellationToken);
        return reviews;
    }
}