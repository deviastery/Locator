namespace Locator.Domain.Vacancies;

public class Review
{
    public Review(double mark, string? comment, string userName, Guid vacancyId)
    {
        Mark = mark;
        Comment = comment;
        UserName = userName;
        VacancyId = vacancyId;
    }
    public Guid Id { get; init; } = Guid.NewGuid();
    public double Mark { get; init; }
    public string? Comment { get; init; }
    public string UserName { get; init; }
    public Guid VacancyId { get; init; }
    public Vacancy? Vacancy { get; private set; } 
    public DateTime CreatedAt { get; init; }
    public static double CalculateAverageMark(List<Review> reviews)
    {
        if (reviews.Count == 0)
            return 0.0;

        return reviews.Average(review => review.Mark);
    }
}