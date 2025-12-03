using Shared.Thesauruses;

namespace Users.Domain;

public class User
{
    public User(long employeeId, string name, string email, RoleType role = RoleType.USER)
    {
        Id = Guid.NewGuid();
        EmployeeId = employeeId;
        Name = name;
        Email = email;
        Role = role;
    }
    public Guid Id { get; set; }
    public long EmployeeId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public RoleType Role { get; set; } 
}