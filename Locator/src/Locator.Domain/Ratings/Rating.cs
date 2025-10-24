using Locator.Domain.Thesauruses;

namespace Locator.Domain.Ratings;

public abstract class Rating
{
    public Rating(double value, long entityId, EntityType entityType)
    {
        Id = Guid.NewGuid();
        Value = value;
        EntityId = entityId;
        EntityType = entityType;
    }
    public Guid Id { get; init; }
        public double Value { get; set; }
    public long EntityId { get; set; }
        public EntityType EntityType { get; set; }
}