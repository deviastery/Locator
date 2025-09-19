using Locator.Domain.Ratings;
using Locator.Domain.Vacancies;
using Locator.Infrastructure.Postgresql.Ratings;
using Locator.Infrastructure.Postgresql.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Infrastructure.Postgresql;

public class LocatorDbContext : DbContext
{
    public LocatorDbContext(DbContextOptions<LocatorDbContext> options)
        : base(options)
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<VacancyRating> VacancyRatings { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new VacanciesConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewsConfiguration());
        modelBuilder.ApplyConfiguration(new RatingsConfiguration());
        
        // определяем вакансии
        Vacancy developer = new Vacancy("Разработчик .Net", "Разрабатывать сервисы на .Net");
        Vacancy tester = new Vacancy("Тестировщик .Net", "Тестировать сервисы на .Net");
        // определяем отзывы
        Review developerReview = new Review(5.0, "Быстро отвечают", "Маша", 
            developer.Id);
        Review testerReview = new Review(3.5, "Медленно отвечают", "Петя", 
            tester.Id);
        // определяем рейтинги
        VacancyRating developerRating = new VacancyRating(5.0, developer.Id);
        developer.RatingId = developerRating.Id;
        VacancyRating testerRating = new VacancyRating(3.5, tester.Id);
        tester.RatingId = testerRating.Id;
        
        // добавляем данные для обеих сущностей
        modelBuilder.Entity<Vacancy>().HasData(developer, tester);
        modelBuilder.Entity<Review>().HasData(developerReview, testerReview);
        modelBuilder.Entity<VacancyRating>().HasData(developerRating, testerRating);
    }
}