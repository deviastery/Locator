using Locator.Domain.Rating;
using Locator.Domain.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Infrastructure.Postgresql;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<VacancyRating> Rating { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Review>().ToTable("reviews");
        modelBuilder.Entity<VacancyRating>().ToTable("vacancyRating");

        modelBuilder.Entity<Review>()
            .Property(r => r.Id)
            .ValueGeneratedNever();
        modelBuilder.Entity<VacancyRating>()
            .Property(r => r.Id)
            .ValueGeneratedNever();
    }
}