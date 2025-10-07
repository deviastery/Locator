namespace Locator.Domain.Users;

public class User
{
    public User(long employeeId, string name, string email, RoleType role = RoleType.USER)
    {
        EmployeeId = employeeId;
        Name = name;
        Email = email;
        Role = role;
        
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public long EmployeeId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public RoleType Role { get; set; } 
}