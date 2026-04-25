using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vacancies.Domain;

namespace Vacancies.Infrastructure.Postgresql;

public class ReviewsConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews"); 
        
        builder.Property(r => r.Id)
            .ValueGeneratedNever();
        builder.Property(r => r.Mark)
            .IsRequired();
        builder.HasCheckConstraint("CK_Review_Mark_Range", "\"Mark\" >= 0 AND \"Mark\" <= 5");
        builder.Property(r => r.Comment)
            .HasMaxLength(250);
        builder.Property(r => r.UserName)
            .HasMaxLength(15)
            .IsRequired();
        builder.Property(r => r.VacancyId)
            .IsRequired();
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();
    }
}