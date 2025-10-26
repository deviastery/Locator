using CSharpFunctionalExtensions;
using Shared;

namespace Locator.Domain.Vacancies;

public class Review
{
    public Review(double mark, string? comment, Guid userId, string userName, long vacancyId)
    {
        Id = Guid.NewGuid();
        Mark = mark;
        Comment = comment;
        UserId = userId;
        UserName = userName;
        VacancyId = vacancyId;
    }
    public Guid Id { get; init; }
    public double Mark { get; init; }
    public string? Comment { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; }
    public long VacancyId { get; init; }
    public DateTime CreatedAt { get; init; }
    public static Result<double, Error> CalculateAverageMark(List<Review> reviews)
    {
        try
        {
            double average = reviews.Average(review => review.Mark);
            return average;
        }
        catch (Exception ex)
        {
            return Error.Failure($"Failed to calculate average: {ex.Message}", "calculation.failed");
        }
    }
}