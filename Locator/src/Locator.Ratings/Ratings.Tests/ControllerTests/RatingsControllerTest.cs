using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Ratings.Application.UpdateVacancyRatingCommand;
using Ratings.Contracts.Dto;
using Ratings.Presenters;
using Shared.Abstractions;

namespace Ratings.Test.ControllerTests;

public class RatingsControllerTest
{
    private readonly RatingsController _ratingsController;
    private readonly Fixture _fixture;
    
    public RatingsControllerTest()
    {
        _ratingsController = new RatingsController();
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task Should_not_have_error_in_UpdateVacancyRating()
    {
        // Arrange
        var commandHandlerMock = new Mock<ICommandHandler<Guid, UpdateVacancyRatingCommand>>();
        var request = _fixture.Build<VacancyRatingValueDto>()
            .With(m => m.AverageMark, () => _fixture.Create<double>() % 5.0)
            .Create();
        long vacancyId = _fixture.Create<long>();
        
        // Act
        var expectedResponse = _fixture.Create<Guid>();
        commandHandlerMock
            .Setup(h => h.Handle(
                It.IsAny<UpdateVacancyRatingCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        
        var result = (OkObjectResult)await _ratingsController.UpdateVacancyRating(
            commandHandlerMock.Object,
            vacancyId,
            request,
            CancellationToken.None);
        
        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Should().BeOfType<OkObjectResult>();
        result.Value.Should().BeOfType<Guid>();
        result.Value.Should().Be(expectedResponse);
    } 
}