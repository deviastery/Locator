using Locator.Domain.Thesauruses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Locator.Infrastructure.Postgresql.Ratings;

public class RatingsConfiguration : IEntityTypeConfiguration<Domain.Ratings.Rating>
{
    public void Configure(EntityTypeBuilder<Domain.Ratings.Rating> builder)
    {
        var ratingEntityTypes = string.Join(", ", Enum.GetNames<EntityType>().Select(name => $"'{name}'"));
        
        builder.UseTpcMappingStrategy();
        
        builder
            .Property(r => r.Id)
            .ValueGeneratedNever();
        builder
            .Property(r => r.Value)
            .IsRequired();
        builder
            .HasCheckConstraint("CK_Rating_Value_Range", "\"Value\" >= 0 AND \"Value\" <= 5");
        builder
            .Property(r => r.EntityId)
            .IsRequired();
        builder.Property(r => r.EntityType)
            .HasConversion<string>()
            .IsRequired();
        builder.HasCheckConstraint(
            "CK_Rating_EntityType_Valid",
            $"\"EntityType\" IN ({ratingEntityTypes})");
    }
}