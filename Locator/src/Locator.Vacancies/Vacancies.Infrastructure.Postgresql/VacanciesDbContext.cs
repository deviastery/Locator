using Microsoft.EntityFrameworkCore;
using Vacancies.Application;
using Vacancies.Domain;

namespace Vacancies.Infrastructure.Postgresql;

public class VacanciesDbContext : DbContext, IVacanciesReadDbContext
{
    public VacanciesDbContext(DbContextOptions<VacanciesDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<Review> Reviews { get; set; }
    public IQueryable<Review> ReadReviews => Reviews.AsNoTracking().AsQueryable();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ReviewsConfiguration());
    }
}