using Locator.Domain.Rating;
using Locator.Domain.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Infrastructure.Postgresql;

public class LocatorDbContext : DbContext
{
    public LocatorDbContext(DbContextOptions<LocatorDbContext> options)
        : base(options)
    {
    }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<VacancyRating> VacancyRatings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Связь Vacancy → Reviews (один ко многим)
        modelBuilder.Entity<Vacancy>()
            .HasMany(v => v.Reviews)
            .WithOne()
            .HasForeignKey(r => r.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Vacancy>().ToTable("vacancies");
        modelBuilder.Entity<Review>().ToTable("reviews");
        modelBuilder.Entity<VacancyRating>().ToTable("vacancy_ratings");

        modelBuilder.Entity<Vacancy>()
            .Property(r => r.Id)
            .ValueGeneratedNever();
        modelBuilder.Entity<Review>()
            .Property(r => r.Id)
            .ValueGeneratedNever();
        modelBuilder.Entity<VacancyRating>()
            .Property(r => r.Id)
            .ValueGeneratedNever();
    }
}