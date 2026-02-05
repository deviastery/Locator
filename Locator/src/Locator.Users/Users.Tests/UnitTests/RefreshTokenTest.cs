using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using Shared;
using Users.Application.JwtTokens;
using Users.Application.RefreshTokenCommand;
namespace Users.Tests.UnitTests;

public class RefreshAccessTokenTest
{
    private readonly Mock<IJwtProvider> _jwtProviderMock;
    private readonly RefreshTokenCommandHandler _handler;
    
    public RefreshAccessTokenTest()
    {
        _jwtProviderMock = new Mock<IJwtProvider>();
        _handler = new RefreshTokenCommandHandler(_jwtProviderMock.Object);
    }
    
    [Fact]
    public async Task Should_not_have_error_in_RefreshTokenCommandHandler()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RefreshTokenCommand(userId);
        
        _jwtProviderMock
            .Setup(x => x.RefreshAccessTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<string, Error>("jwt-token"));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<string>();
        result.Value.Should().NotBeNullOrEmpty();
    } 
}