using CSharpFunctionalExtensions;
using Locator.Application.Vacancies;
using Locator.Application.Vacancies.Fails;
using Locator.Domain.Vacancies;
using Microsoft.EntityFrameworkCore;
using Shared;
using Errors = Locator.Application.Users.Fails.Errors;

namespace Locator.Infrastructure.Postgresql.Vacancies;

public class VacanciesEfCoreRepository : IVacanciesRepository
{
    private readonly VacanciesDbContext _vacanciesDbContext;

    public VacanciesEfCoreRepository(VacanciesDbContext vacanciesDbContext)
    {
        _vacanciesDbContext = vacanciesDbContext;
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