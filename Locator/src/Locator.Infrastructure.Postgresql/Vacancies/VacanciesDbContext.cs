using Locator.Application.Vacancies;
using Locator.Domain.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Infrastructure.Postgresql.Vacancies;

public class VacanciesDbContext : DbContext, IVacanciesReadDbContext
{
    public VacanciesDbContext(DbContextOptions<VacanciesDbContext> options)
        : base(options)
    {
    }
    public DbSet<Review> Reviews { get; set; }
    public IQueryable<Review> ReadReviews => Reviews.AsNoTracking().AsQueryable();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ReviewsConfiguration());
    }
}