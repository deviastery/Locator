namespace Locator.Domain.Vacancies;

public class Vacancy
{
    public Vacancy(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }
    public int? Salary { get; set; }
    public int? Experience { get; set; }
    public Guid? RatingId { get; set; }
    public List<Review>? Reviews { get; set; } = [];
}