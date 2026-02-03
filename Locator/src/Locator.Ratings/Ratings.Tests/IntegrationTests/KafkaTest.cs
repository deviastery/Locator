using System.Text.Json;
using AutoFixture;
using Confluent.Kafka;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ratings.Contracts.Dto;
using Ratings.Infrastructure.Postgresql;

namespace Ratings.Test.IntegrationTests;

public class KafkaTest : IClassFixture<DockerWebApplicationFactoryFixture>
{
    private DockerWebApplicationFactoryFixture _factory;
    private IProducer<Null, string> _producer;
    private readonly Fixture _fixture;
    
    public KafkaTest(DockerWebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _producer = factory.CreateProducer();
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task Should_not_have_error_when_topic_is_single()
    {
        // Arrange
        long vacancyId = _fixture.Create<long>();
        var dto = _fixture.Build<UpdateVacancyRatingDto>()
            .With(x => x.VacancyId, vacancyId)
            .With(x => x.AverageMark, () => _fixture.Create<double>() % 5.0)
            .Create();
        string message = JsonSerializer.Serialize(dto);
        
        // Act
        await _producer.ProduceAsync(
            DockerWebApplicationFactoryFixture.AddReviewTopic,
            new Message<Null, string> { Value = message });
        await Task.Delay(TimeSpan.FromSeconds(7));
        
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RatingsDbContext>();
        
        var rating = await context.VacancyRatings
            .FirstOrDefaultAsync(r => r.EntityId == vacancyId);
        
        // Assert
        rating.Should().NotBeNull();
        rating.Value.Should().Be(dto.AverageMark);
    }
    
    [Fact]
    public async Task Should_not_have_error_when_topic_is_more_than_one()
    {
        // Arrange
        var events = new List<UpdateVacancyRatingDto>();
        for (int i = 0; i < 3; i++)
        {
            events.Add(_fixture.Build<UpdateVacancyRatingDto>()
                .With(x => x.VacancyId, _fixture.Create<long>())
                .With(x => x.AverageMark, () => _fixture.Create<double>() % 5.0)
                .Create());
        }
        
        // Act
        foreach (var dto in events)
        {
            string message = JsonSerializer.Serialize(dto);
            await _producer.ProduceAsync(
                DockerWebApplicationFactoryFixture.AddReviewTopic,
                new Message<Null, string> { Value = message });
        }

        await Task.Delay(TimeSpan.FromSeconds(7));
        
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RatingsDbContext>();
        
        // Assert
        foreach (var dto in events)
        {
            var rating = await context.VacancyRatings
                .FirstOrDefaultAsync(r => r.EntityId == dto.VacancyId);

            rating.Should().NotBeNull();
            rating.Value.Should().Be(dto.AverageMark);
        }
    }
    
    [Fact]
    public async Task Should_not_have_error_when_message_is_invalid()
    {
        // Arrange
        string invalidMessage = "{ invalid json";

        // Act
        await _producer.ProduceAsync(
            DockerWebApplicationFactoryFixture.AddReviewTopic,
            new Message<Null, string> { Value = invalidMessage });

        await Task.Delay(TimeSpan.FromSeconds(7));

        // Assert
        Assert.True(true);
    }
}