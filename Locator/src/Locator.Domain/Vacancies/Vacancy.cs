namespace Locator.Domain.Vacancies;

public class Vacancy
{
    public Vacancy(string name, string description, string employeeId)
    {
        Name = name;
        Description = description;
        EmployeeId = employeeId;
    }
    public Guid Id { get; init; } = Guid.NewGuid();
    public string EmployeeId { get; init; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? Salary { get; set; }
    public int? Experience { get; set; }
    public Guid? RatingId { get; set; }
    public List<Review>? Reviews { get; set; } = [];
}