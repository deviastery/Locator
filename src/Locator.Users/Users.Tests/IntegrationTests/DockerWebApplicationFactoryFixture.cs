using AutoFixture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Users.Domain;
using Users.Infrastructure.Postgresql;
using Users.Web;

namespace Users.Tests.IntegrationTests;

public class DockerWebApplicationFactoryFixture : WebApplicationFactory<Program>,
    IAsyncLifetime
{
    private PostgreSqlContainer _dbContainer;
    private readonly Fixture _fixture;

    public int InitialUsersCount = 3;
    
    public DockerWebApplicationFactoryFixture()
    {
        _dbContainer = new PostgreSqlBuilder().Build();
        _fixture = new Fixture();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        string? connectionString = _dbContainer.GetConnectionString();
        
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<UsersDbContext>));
            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using (var scope = Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var cntx = scopedServices.GetRequiredService<UsersDbContext>();
            
            await cntx.Database.EnsureCreatedAsync();

            var users = _fixture.Build<User>()
                .CreateMany(InitialUsersCount)
                .ToList();
            await cntx.Users.AddRangeAsync(users);
            await cntx.SaveChangesAsync();
        }
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}