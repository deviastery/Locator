namespace Locator.Domain.Vacancies;

public class Vacancy
{
    public Vacancy(long id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
    public long Id { get; init; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? Salary { get; set; }
    public int? Experience { get; set; }
    public Guid? RatingId { get; set; }
    public List<Review>? Reviews { get; set; } = [];
}