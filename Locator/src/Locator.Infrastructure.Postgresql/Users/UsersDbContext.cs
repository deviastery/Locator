using Locator.Application.Users;
using Locator.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Locator.Infrastructure.Postgresql.Users;

public class UsersDbContext : DbContext, IUsersReadDbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public IQueryable<User> ReadUsers => Users.AsNoTracking().AsQueryable();
    public DbSet<UserSession> UserSessions { get; set; }
    public IQueryable<UserSession> ReadUserSessions => UserSessions.AsNoTracking().AsQueryable();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UsersConfiguration());
    }
}