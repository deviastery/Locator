using AutoFixture;
using FluentAssertions;
using Redis.Contracts;
using Redis.Contracts.Dto;

namespace Redis.Tests.IntegrationTests;

public class RedisTest : IClassFixture<DockerRedisFixture>
{
    private DockerRedisFixture _fixture;
    private ITokenCacheContract _tokenCache;
    private readonly Fixture _autoFixture;
    
    public RedisTest(DockerRedisFixture fixture)
    {
        _fixture = fixture;
        _tokenCache = _fixture.GetTokenCache();
        _autoFixture = new Fixture();
    }
    
    [Fact]
    public async Task Should_not_have_error_when_EmployeeToken_is_valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string? token = _autoFixture.Create<string>();
        
        // Act
        await _tokenCache.SetEmployeeTokenAsync(token, userId, CancellationToken.None);
        string? cachedToken = await _tokenCache.GetEmployeeTokenAsync(userId, CancellationToken.None);
        
        // Assert
        cachedToken.Should().NotBeNull();
        cachedToken.Should().Be(token);
    }
    
    [Fact]
    public async Task Should_not_have_error_when_RefreshToken_is_valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = _autoFixture.Build<RefreshTokenDto>()
            .With(x => x.UserId, userId)
            .Create();
        
        // Act
        await _tokenCache.SetRefreshTokenAsync(token, CancellationToken.None);
        var cachedToken = await _tokenCache.GetRefreshTokenAsync(userId, CancellationToken.None);
        
        // Assert
        cachedToken.Should().NotBeNull();
        cachedToken.Should().Be(token);
    }
}