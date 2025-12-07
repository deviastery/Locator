using Microsoft.EntityFrameworkCore;
using Users.Application;
using Users.Domain;

namespace Users.Infrastructure.Postgresql;

public class UsersDbContext : DbContext, IUsersReadDbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<User> Users { get; set; }
    public IQueryable<User> ReadUsers => Users.AsNoTracking().AsQueryable();
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public IQueryable<RefreshToken> ReadRefreshTokens => RefreshTokens.AsNoTracking().AsQueryable();
    public DbSet<EmployeeToken> EmployeeTokens { get; set; }
    public IQueryable<EmployeeToken> ReadEmployeeTokens => EmployeeTokens.AsNoTracking().AsQueryable();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UsersConfiguration());
        modelBuilder.ApplyConfiguration(new TokensConfiguration());
    }
}