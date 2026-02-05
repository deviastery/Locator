using System.Security.Claims;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Abstractions;
using Vacancies.Application.CreateReviewCommand;
using Vacancies.Contracts.Dto;
using Vacancies.Presenters;

namespace Vacancies.Test.ControllerTests;

public class CreateReviewTest
{
    private readonly VacanciesController _vacanciesController;
    private readonly Fixture _fixture;
    
    public CreateReviewTest()
    {
        _vacanciesController = new VacanciesController();
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task Should_have_error_in_CreateReview_without_authorization()
    {
        // Arrange
        var commandHandlerMock = new Mock<ICommandHandler<Guid, CreateReviewCommand>>();
        var request = _fixture.Build<CreateReviewDto>()
            .With(r => r.Mark, () => _fixture.Create<double>() % 5.0)
            .Create();
        long vacancyId = _fixture.Create<long>();
        
        // Create unauthorized user
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims); 
        var claimsPrincipal = new ClaimsPrincipal(identity);
    
        // Set unauthorized user in http context
        _vacanciesController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal,
            },
        };
        
        var expectedResponse = _fixture.Create<Guid>();
        
        commandHandlerMock
            .Setup(h => h.Handle(
                It.IsAny<CreateReviewCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        
        // Act
        var result = (UnauthorizedResult)await _vacanciesController.CreateReview(
            commandHandlerMock.Object,
            vacancyId,
            request,
            CancellationToken.None);
        
        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        result.Should().BeOfType<UnauthorizedResult>();
    } 
}