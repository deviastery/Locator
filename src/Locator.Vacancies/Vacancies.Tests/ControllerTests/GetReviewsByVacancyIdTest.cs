using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Abstractions;
using Vacancies.Application.GetReviewsByVacancyIdQuery;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;
using Vacancies.Presenters;

namespace Vacancies.Test.ControllerTests;

public class GetReviewsByVacancyIdTest
{
    private readonly VacanciesController _vacanciesController;
    private readonly Fixture _fixture;
    
    public GetReviewsByVacancyIdTest()
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
        
        var expectedResponse = new ReviewsByVacancyIdResponse(reviews);
        
        queryHandlerMock
            .Setup(h => h.Handle(
                It.IsAny<GetReviewsByVacancyIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        
        // Act
        var result = (OkObjectResult)await _vacanciesController.GetReviewsByVacancyId(
            queryHandlerMock.Object,
            vacancyId,
            CancellationToken.None);
        
        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Should().NotBeNull();
    }
}