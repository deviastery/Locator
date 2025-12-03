using Microsoft.EntityFrameworkCore;
using Ratings.Application;
using Ratings.Domain;

namespace Ratings.Infrastructure.Postgresql;

public class RatingsDbContext : DbContext, IRatingsReadDbContext
{
    public RatingsDbContext(DbContextOptions<RatingsDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<VacancyRating> VacancyRatings { get; set; }
    public IQueryable<VacancyRating> ReadVacancyRatings => VacancyRatings.AsNoTracking().AsQueryable();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new RatingsConfiguration());
    }
}