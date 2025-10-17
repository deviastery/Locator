namespace Locator.Contracts.Vacancies;

public record FullVacancyDto
{
    public FullVacancyDto(long id, VacancyDto dto, double? rating)
    {
        Id = id;
        Name = dto.Name;
        Description = dto.Description?.Responsibility;
        Employer = dto.Employer?.Name;
        Area = dto.Area;
        Address = dto.Address;
        Experience = dto.Experience;
        Salary = dto.Salary;
        Schedule = dto.Schedule;
        WorkFormat = dto.WorkFormat;
        Rating = rating;
    }

    public long Id { get; init; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public string? Employer { get; init; }
    public Area? Area { get; init; }
    public Address? Address { get; init; }
    public Experience? Experience { get; init; }
    public Salary? Salary { get; init; }
    public Schedule? Schedule { get; init; }
    public List<WorkFormat>? WorkFormat { get; init; }
    public double? Rating { get; init; }
}