using AutoFixture;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ratings.Domain;
using Ratings.Infrastructure.Postgresql;
using Ratings.Web;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;

namespace Ratings.Test.IntegrationTests;

public class DockerWebApplicationFactoryFixture : WebApplicationFactory<Program>,
    IAsyncLifetime
{
    private KafkaContainer _kafkaContainer;
    private PostgreSqlContainer _dbContainer;
    private readonly Fixture _fixture;
    
    public const string AddReviewTopic = "add-review";    
    public int InitialVacancyRatingsCount = 3;
    
    public DockerWebApplicationFactoryFixture()
    {
        _kafkaContainer = new KafkaBuilder().Build();
        _dbContainer = new PostgreSqlBuilder().Build();
        _fixture = new Fixture();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        string? kafkaBootstrapServers = _kafkaContainer.GetBootstrapAddress();
        var dbConnectionString = _dbContainer.GetConnectionString();
        
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<RatingsDbContext>));
            services.AddDbContext<RatingsDbContext>(options =>
            {
                options.UseNpgsql(dbConnectionString);
            });
            
            services.RemoveAll(typeof(IConsumer<Null, string>));
            services.AddSingleton<IConsumer<Null, string>>(sp =>
            {
                var config = new ConsumerConfig
                {
                    GroupId = "test-consumer-group",
                    BootstrapServers = kafkaBootstrapServers,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false,
                };
                return new ConsumerBuilder<Null, string>(config).Build();
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _kafkaContainer.StartAsync();
        await _dbContainer.StartAsync();
        
        await CreateTopicAsync(AddReviewTopic);

        using (var scope = Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var cntx = scopedServices.GetRequiredService<RatingsDbContext>();
            
            await cntx.Database.EnsureCreatedAsync();

            var ratings = _fixture.Build<VacancyRating>()
                .With(x => x.Value, () => _fixture.Create<double>() % 5.0)
                .CreateMany(InitialVacancyRatingsCount)
                .ToList();
            await cntx.VacancyRatings.AddRangeAsync(ratings);
            await cntx.SaveChangesAsync();
        }
    }
    
    public IProducer<Null, string> CreateProducer()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaContainer.GetBootstrapAddress(),
        };
        return new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _kafkaContainer.StopAsync();
    }
    
    private async Task CreateTopicAsync(string topicName)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _kafkaContainer.GetBootstrapAddress(),
        }).Build();

        await adminClient.CreateTopicsAsync([
            new TopicSpecification
            {
                Name = topicName,
                NumPartitions = 1,
                ReplicationFactor = 1,
            }
        ]);
    }
}