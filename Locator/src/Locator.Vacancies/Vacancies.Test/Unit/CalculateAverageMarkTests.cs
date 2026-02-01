using AutoFixture;
using FluentAssertions;
using Vacancies.Domain;

namespace Vacancies.Test;

public class CalculateAverageMarkTests
{
    private readonly Fixture _fixture;
 
    public CalculateAverageMarkTests()
    {
        _fixture = new Fixture();
    }
    
    [Fact]
    public void Should_not_have_error_when_Reviews_is_valid()
    {
        // Arrange
        var reviews = _fixture.Build<Review>()
            .With(r => r.Mark, () => _fixture.Create<double>() % 5.0)
            .CreateMany(3)
            .ToList();
        double expectedAverage = reviews.Average(r => r.Mark);
        
        // Act
        double result = Review.CalculateAverageMark(reviews).Value;
        
        // Assert
        result.Should().BeApproximately(expectedAverage, 0.01);
    } 
    
    [Fact]
    public void Should_not_have_error_when_Marks_are_zero()
    {
        // Arrange
        var reviews = _fixture.Build<Review>()
            .With(r => r.Mark, 0.0)
            .CreateMany(3)
            .ToList();
        double expectedAverage = 0.0;
        
        // Act
        double result = Review.CalculateAverageMark(reviews).Value;
        
        // Assert
        result.Should().BeApproximately(expectedAverage, 0.01);
    }
}