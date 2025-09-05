namespace Locator.Domain.Users;

public class User
{
    public User(RoleType role)
    {
        Role = role;
    }
    public Guid Id { get; set; }
    public RoleType Role { get; set; }
}