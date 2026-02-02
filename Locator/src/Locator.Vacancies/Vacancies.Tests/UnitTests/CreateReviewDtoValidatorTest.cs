using AutoFixture;
using FluentValidation.TestHelper;
using Vacancies.Application.CreateReviewCommand;
using Vacancies.Contracts.Dto;

namespace Vacancies.Test.UnitTests;

public class CreateReviewDtoValidatorTest
{
    private readonly CreateReviewDtoValidator _validator;
    private readonly Fixture _fixture;
 
    public CreateReviewDtoValidatorTest()
    {
        _validator = new CreateReviewDtoValidator();
        _fixture = new Fixture();
    }
    
    [Theory]
    [InlineData(0.0)]
    [InlineData(2.5)]
    [InlineData(5.0)]
    public void Should_not_have_error_when_Mark_is_valid(double mark)
    {
        // Arrange
        var dto = new CreateReviewDto(mark);
        
        // Act
        var result = _validator.TestValidate(dto);
        
        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Mark);
    }
    
    [Fact]
    public void Should_not_have_error_when_Comment_is_valid()
    {
        // Arrange
        double mark = 4.0;
        string comment = string.Join(string.Empty, _fixture.CreateMany<char>(100));
        var dto = new CreateReviewDto(mark, comment);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }
    
    [Fact]
    public void Should_have_error_when_Mark_less_than_zero()
    {
        // Arrange
        double mark = -1.0;
        var dto = new CreateReviewDto(mark);
        
        // Act
        var result = _validator.TestValidate(dto);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Mark);
    }    
    
    [Fact]
    public void Should_have_error_when_Mark_greater_than_five()
    {
        // Arrange
        double mark = 6.0;
        var dto = new CreateReviewDto(mark);
        
        // Act
        var result = _validator.TestValidate(dto);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Mark);
    }
    
    [Fact]
    public void Should_have_error_when_Comment_is_longer_than_250_characters()
    {
        // Arrange
        double mark = 4.0;
        string comment = string.Join(string.Empty, _fixture.CreateMany<char>(251));
        var dto = new CreateReviewDto(mark, comment);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Comment);
    }
}