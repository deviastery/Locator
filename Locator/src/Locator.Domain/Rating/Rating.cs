namespace Locator.Domain.Rating;

public abstract class Rating
{
    public Rating(double value, Guid entityId, EntityType entityType)
    {
        Value = value;
        EntityId = entityId;
        EntityType = entityType;
    }
    public Guid Id { get; init; } = Guid.NewGuid();
    public double Value { get; private set; }
    public Guid EntityId { get; private set; }
    public EntityType EntityType { get; private set; }
}