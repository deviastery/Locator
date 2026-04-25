using Shared.Thesauruses;

namespace Ratings.Domain;

public class VacancyRating: Rating
{
    public VacancyRating(double value, long entityId)
        : base(value, entityId, EntityType.VACANCY)
    {
    }
}