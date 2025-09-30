using Locator.Domain.Thesauruses;

namespace Locator.Domain.Ratings;

public abstract class Rating
{
    public Rating(double? value, Guid entityId, EntityType entityType)
    {
        Value = value;
        EntityId = entityId;
        EntityType = entityType;
    }
    public Guid Id { get; init; } = Guid.NewGuid();
    public double Value { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
}