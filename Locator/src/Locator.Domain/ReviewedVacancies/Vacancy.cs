namespace Locator.Domain.Reviews;

public class Vacancy
{
    public Vacancy(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? Salary { get; set; }
    public int? Experience { get; set; }
    public double Rating { get; set; } = 0;
    public List<Review>? Reviews { get; set; } = [];
}