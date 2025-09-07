namespace Locator.Domain.Vacancies;

public class Review
{
    public Review(double mark, string? comment, Guid userId, Guid vacancyId)
    {
        Mark = mark;
        Comment = comment;
        UserId = userId;
        VacancyId = vacancyId;
    }
    public Guid Id { get; init; } = Guid.NewGuid();
    public double Mark { get; init; }
    public string? Comment { get; set; }
    public Guid UserId { get; init; }
    public Guid VacancyId { get; init; }
    public static double CalculateAverageMark(List<Review> reviews)
    {
        if (reviews.Count == 0)
            return 0.0;

        return reviews.Average(review => review.Mark);
    }
}