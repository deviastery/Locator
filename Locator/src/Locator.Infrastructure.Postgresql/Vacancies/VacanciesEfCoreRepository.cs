using CSharpFunctionalExtensions;
using Locator.Application.Vacancies;
using Locator.Application.Vacancies.Fails;
using Locator.Application.Vacancies.GetVacanciesWithFilters;
using Locator.Domain.Vacancies;
using Microsoft.EntityFrameworkCore;
using Shared;

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

    public async Task<List<Review>> GetReviewsByVacancyIdAsync(Guid vacancyId, CancellationToken cancellationToken)
    {
        var reviews = await _vacanciesDbContext.Reviews
            .Where(r => r.VacancyId == vacancyId)
            .ToListAsync(cancellationToken);
        return reviews;
    }
    public async Task<int> GetDaysAfterApplyingAsync(Guid vacancyId, string userName, CancellationToken cancellationToken)
    {
        // TODO: Соответствующий запрос на HH Api + бизнес логика
        return 6;
    }
    public async Task<Result<Vacancy, Error>> GetVacancyByIdAsync(Guid vacancyId, CancellationToken cancellationToken)
    {
        try
        {
            var vacancy = await _vacanciesDbContext.Vacancies
                .Include(v => v.Reviews)
                .FirstOrDefaultAsync(v => v.Id == vacancyId, cancellationToken);
            if (vacancy is null)
            {
                return Errors.General.NotFound(vacancyId);
            }

            return vacancy;
        }
        catch (Exception e)
        {
            return Errors.General.NotFound(vacancyId);
        }
    }
}