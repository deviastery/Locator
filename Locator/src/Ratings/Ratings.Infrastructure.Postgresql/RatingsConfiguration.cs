using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ratings.Domain;
using Shared.Thesauruses;

namespace Ratings.Infrastructure.Postgresql;

public class RatingsConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        string ratingEntityTypes = string.Join(", ", Enum.GetNames<EntityType>().Select(name => $"'{name}'"));
        
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
        builder
            .Property(r => r.EntityType)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("text");
        builder
            .HasCheckConstraint(
                "CK_Rating_EntityType_Valid",
                $"\"EntityType\" IN ({ratingEntityTypes})");
    }
}