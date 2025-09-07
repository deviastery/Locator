namespace Locator.Domain.Rating;

public class Rating
{
    public Rating(double rating, Guid entityId, EntityType entityType)
    {
        Value = rating;
        EntityId = entityId;
        EntityType = entityType;
    }
    public Guid Id { get; init; } = Guid.NewGuid();
    public double Value { get; private set; } = 0;
    public Guid EntityId { get; private set; }
    public EntityType EntityType { get; private set; }
}