using Locator.Application.Vacancies;
using Locator.Domain.Ratings;
using Locator.Domain.Users;
using Locator.Domain.Vacancies;
using Locator.Infrastructure.Postgresql.Ratings;
using Locator.Infrastructure.Postgresql.Users;
using Microsoft.EntityFrameworkCore;
using Guid = System.Guid;

namespace Locator.Infrastructure.Postgresql.Vacancies;

public class VacanciesDbContext : DbContext, IVacanciesReadDbContext
{
    public VacanciesDbContext(DbContextOptions<VacanciesDbContext> options)
        : base(options)
    {
        // Database.EnsureDeleted();
        // Database.EnsureCreated();
    }
    public DbSet<Vacancy> Vacancies { get; set; }
    public IQueryable<Vacancy> ReadVacancies => Vacancies.AsNoTracking().AsQueryable();
    public DbSet<Review> Reviews { get; set; }
    public IQueryable<Review> ReadReviews => Reviews.AsNoTracking().AsQueryable();
    
    public DbSet<VacancyRating> VacancyRatings { get; set; }
    public DbSet<User> Users { get; set; }
    public IQueryable<User> ReadUsers => Users.AsNoTracking().AsQueryable();
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<EmployeeToken> EmployeeTokens { get; set; }
    public IQueryable<EmployeeToken> ReadEmployeeTokens => EmployeeTokens.AsNoTracking().AsQueryable();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new VacanciesConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewsConfiguration());
        modelBuilder.ApplyConfiguration(new RatingsConfiguration());
        modelBuilder.ApplyConfiguration(new UsersConfiguration());
        
        // определяем вакансии
        Vacancy developer = new Vacancy("Разработчик .Net", 
            "Разрабатывать сервисы на .Net", "1");
        Vacancy tester = new Vacancy("Тестировщик .Net", 
            "Тестировать сервисы на .Net", "2");
        // определяем отзывы
        Review developerReview = new Review(5.0, "Быстро отвечают", "Маша", 
            developer.Id.ToString());
        Review testerReview = new Review(3.5, "Медленно отвечают", "Петя", 
            tester.Id.ToString());
        // определяем рейтинги
        VacancyRating developerRating = new VacancyRating(5.0, developer.Id.ToString());
        developer.RatingId = developerRating.Id;
        VacancyRating testerRating = new VacancyRating(3.5, tester.Id.ToString());
        tester.RatingId = testerRating.Id;
        // определяем пользователей
        User user = new User(1, "Паша", "pasha@gmail.com");                    
        RefreshToken token = new RefreshToken(Guid.NewGuid(), DateTime.UtcNow, user.Id);                    
        EmployeeToken employeeToken = new EmployeeToken(user.Id, "123", "456",
            DateTime.UtcNow, 1000);                    
        
        // добавляем данные для обеих сущностей
        modelBuilder.Entity<Vacancy>().HasData(developer, tester);
        modelBuilder.Entity<Review>().HasData(developerReview, testerReview);
        modelBuilder.Entity<VacancyRating>().HasData(developerRating, testerRating);
        modelBuilder.Entity<User>().HasData(user);
        modelBuilder.Entity<EmployeeToken>().HasData(employeeToken);
        modelBuilder.Entity<RefreshToken>().HasData(token);
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Token);
        });
    }
}