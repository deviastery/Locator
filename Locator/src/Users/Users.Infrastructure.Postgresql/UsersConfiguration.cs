using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Thesauruses;
using Users.Domain;

namespace Users.Infrastructure.Postgresql;

public class UsersConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        string userTypes = string.Join(", ", Enum.GetNames<RoleType>().Select(name => $"'{name}'"));
        
        builder
            .Property(u => u.Id)
            .ValueGeneratedNever();
        builder
            .Property(u => u.EmployeeId)
            .IsRequired();
        builder
            .Property(u => u.Name)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(u => u.Email)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .IsRequired();
        builder.HasCheckConstraint(
            "CK_User_UserType_Valid",
            $"\"Role\" IN ({userTypes})");
    }
}