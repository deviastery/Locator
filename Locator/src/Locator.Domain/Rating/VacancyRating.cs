namespace Locator.Domain.Rating;

public class VacancyRating: Rating
{
    public VacancyRating(double value, Guid entityId)
        : base(value, entityId, EntityType.Vacancy)
    {
    }
}