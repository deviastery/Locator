using Locator.Domain.Thesauruses;

namespace Locator.Domain.Ratings;

public class VacancyRating: Rating
{
    public VacancyRating(double value, string entityId)
        : base(value, entityId, EntityType.VACANCY)
    {
    }
}