using Locator.Domain.Rating;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Locator.Infrastructure.Postgresql.Rating;

public class RatingsConfiguration : IEntityTypeConfiguration<Domain.Rating.Rating>
{
    public void Configure(EntityTypeBuilder<Domain.Rating.Rating> builder)
    {
        var ratingEntityTypes = string.Join(", ", Enum.GetNames<EntityType>().Select(name => $"'{name}'"));
        
        builder.UseTpcMappingStrategy();
        
        builder
            .Property(r => r.Id)
            .ValueGeneratedNever();
        builder
            .Property(u => u.Value)
            .IsRequired();
        builder
            .HasCheckConstraint("CK_Rating_Value_Range", "\"Value\" >= 0 AND \"Value\" <= 5");
        builder
            .Property(u => u.EntityId)
            .IsRequired();
        builder.Property(u => u.EntityType)
            .HasConversion<string>()
            .IsRequired();
        builder.HasCheckConstraint(
            "CK_Rating_EntityType_Valid",
            $"\"EntityType\" IN ({ratingEntityTypes})");
    }
}