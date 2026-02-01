using System.Security.Claims;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Abstractions;
using Vacancies.Application.CreateReviewCommand;
using Vacancies.Application.GetReviewsByVacancyIdQuery;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;
using Vacancies.Presenters;

namespace Vacancies.Test.Controller;

public class VacanciesControllerTest
{
    private readonly VacanciesController _vacanciesController;
    private readonly Fixture _fixture;
    
    public VacanciesControllerTest()
    {
        _vacanciesController = new VacanciesController();
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task Should_not_have_error_in_GetReviewsByVacancyId()
    {
        // Arrange
        var queryHandlerMock = new Mock<IQueryHandler<
            ReviewsByVacancyIdResponse, 
            GetReviewsByVacancyIdQuery>>();
        var reviews = _fixture.Build<ReviewDto>()
            .With(r => r.Mark, () => _fixture.Create<double>() % 5.0)
            .CreateMany(3)
            .ToList();
        long vacancyId = _fixture.Create<long>();
        
        // Act
        var expectedResponse = new ReviewsByVacancyIdResponse(reviews);
        queryHandlerMock
            .Setup(h => h.Handle(
                It.IsAny<GetReviewsByVacancyIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        
        var result = (OkObjectResult)await _vacanciesController.GetReviewsByVacancyId(
            queryHandlerMock.Object,
            vacancyId,
            CancellationToken.None);
        
        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Should().NotBeNull();
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
        
        // Act
        var expectedResponse = _fixture.Create<Guid>();
        commandHandlerMock
            .Setup(h => h.Handle(
                It.IsAny<CreateReviewCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        
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