using Locator.Application.Vacancies;
using Locator.Domain.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Infrastructure.Postgresql.Vacancies;

public class VacanciesDbContext : DbContext, IVacanciesReadDbContext
{
    public VacanciesDbContext(DbContextOptions<VacanciesDbContext> options)
        : base(options)
    {
        // Database.EnsureDeleted();
        // Database.EnsureCreated();
    }
    public DbSet<Review> Reviews { get; set; }
    public IQueryable<Review> ReadReviews => Reviews.AsNoTracking().AsQueryable();
    
    
    
    // public DbSet<VacancyRating> VacancyRatings { get; set; }
    // public DbSet<User> Users { get; set; }
    // public IQueryable<User> ReadUsers => Users.AsNoTracking().AsQueryable();
    // public DbSet<RefreshToken> RefreshTokens { get; set; }
    // public DbSet<EmployeeToken> EmployeeTokens { get; set; }
    // public IQueryable<EmployeeToken> ReadEmployeeTokens => EmployeeTokens.AsNoTracking().AsQueryable();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new ReviewsConfiguration());
        
        
        
        // modelBuilder.ApplyConfiguration(new RatingsConfiguration());
        // modelBuilder.ApplyConfiguration(new UsersConfiguration());
        // modelBuilder.ApplyConfiguration(new TokensConfiguration());
        //
        // // сиды
        // // определяем вакансии
        //
        // // определяем рейтинги
        // VacancyRating developerRating = new VacancyRating(5.0, 1);
        // VacancyRating testerRating = new VacancyRating(3.5, 2);
        //
        // // определяем отзывы
        // Review developerReview = new Review(5.0, "Быстро отвечают", "Маша", 
        //     1);
        // Review testerReview = new Review(3.5, "Медленно отвечают", "Петя", 
        //     2);
        //
        // // определяем пользователей
        // User user = new User(1, "Паша", "pasha@gmail.com");                    
        // RefreshToken token = new RefreshToken(Guid.NewGuid().ToString(), DateTime.UtcNow, 1000, user.Id);                    
        // EmployeeToken employeeToken = new EmployeeToken(token: "123", "456", 
        //     DateTime.UtcNow, 1000, user.Id);                    
        //
        // // добавляем данные для обеих сущностей
        // modelBuilder.Entity<Review>().HasData(developerReview, testerReview);
        // modelBuilder.Entity<VacancyRating>().HasData(developerRating, testerRating);
        // modelBuilder.Entity<User>().HasData(user);
        // modelBuilder.Entity<EmployeeToken>().HasData(employeeToken);
        // modelBuilder.Entity<RefreshToken>().HasData(token);
    }
}