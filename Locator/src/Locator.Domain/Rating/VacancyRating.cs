namespace Locator.Domain.Rating;

public class VacancyRating: Rating
{
    public VacancyRating(double rating, Guid vacancyId)
        : base(rating, vacancyId, EntityType.Vacancy)
    {
    }
}