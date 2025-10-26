using Locator.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Locator.Infrastructure.Postgresql.Users;

public class TokensConfiguration : IEntityTypeConfiguration<BaseToken>
{
    public void Configure(EntityTypeBuilder<BaseToken> builder)
    {
        builder.UseTpcMappingStrategy();
    }
}