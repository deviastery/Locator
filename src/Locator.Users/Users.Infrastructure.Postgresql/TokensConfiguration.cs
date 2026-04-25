using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain;

namespace Users.Infrastructure.Postgresql;

public class TokensConfiguration : IEntityTypeConfiguration<BaseToken>
{
    public void Configure(EntityTypeBuilder<BaseToken> builder)
    {
        builder.UseTpcMappingStrategy();
    }
}