using Locator.Application.Ratings;
using Locator.Domain.Ratings;
using Locator.Domain.Thesauruses;
using Locator.Domain.Vacancies;
using Microsoft.EntityFrameworkCore;
using Guid = System.Guid;

namespace Locator.Infrastructure.Postgresql.Ratings;

public class RatingsDbContext : DbContext, IRatingsReadDbContext
{
    public RatingsDbContext(DbContextOptions<RatingsDbContext> options)
        : base(options)
    {
    }
    public DbSet<VacancyRating> VacancyRatings { get; set; }
    public IQueryable<VacancyRating> ReadVacancyRatings => VacancyRatings.AsNoTracking().AsQueryable();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new RatingsConfiguration());
    }
}