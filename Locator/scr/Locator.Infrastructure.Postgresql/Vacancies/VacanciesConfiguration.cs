using Locator.Domain.Rating;
using Locator.Domain.Vacancies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Locator.Infrastructure.Postgresql.Vacancies;

public class VacanciesConfiguration : IEntityTypeConfiguration<Vacancy>
{
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        // Связь Vacancy -> Rating (один к одному)
        builder
            .HasOne<VacancyRating>()
            .WithOne()
            .HasForeignKey<Vacancy>(v => v.RatingId);
        
        builder
            .Property(r => r.Id)
            .ValueGeneratedNever();
    }
}