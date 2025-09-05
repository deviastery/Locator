using Locator.Domain.Vacancies;

namespace Locator.Domain.Reviews;

public class Review
{
    public Review(double rating, Guid userId, Vacancy vacancy)
    {
        Rating = rating;
        UserId = userId;
        Vacancy = vacancy;
    }
    public Guid Id { get; set; }
    public double Rating { get; set; }
    public string? Comment { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Vacancy Vacancy { get; set; }
}