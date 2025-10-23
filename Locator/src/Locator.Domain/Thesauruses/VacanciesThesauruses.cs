namespace Locator.Domain.Thesauruses;

public enum CurrencyEnum
{
    AZN,
    BYR,
    EUR,
    GEL,
    KGS,
    KZT,
    RUR,
    UAH,
    USD,
    UZS,
}
public enum ExperienceEnum
{
    /// <summary>
    /// No Experience
    /// </summary>
    noExperience,
    /// <summary>
    /// Experience between 1 And 3 years
    /// </summary>
    between1And3,
    /// <summary>
    /// Experience between 3 And 6 years
    /// </summary>
    between3And6,
    /// <summary>
    /// Experience more 6 years
    /// </summary>
    moreThan6,
}
public enum EmploymentEnum
{
    /// <summary>
    /// Employment full-time
    /// </summary>
    full,
    /// <summary>
    /// Employment part-time
    /// </summary>
    part,
    /// <summary>
    /// project Employment
    /// </summary>
    project,
    /// <summary>
    /// volunteer Employment
    /// </summary>
    volunteer,
    /// <summary>
    /// probation Employment
    /// </summary>
    probation,
}
public enum ScheduleEnum
{
    /// <summary>
    /// full day Schedule
    /// </summary>
    fullDay,
    /// <summary>
    /// shift Schedule
    /// </summary>
    shift,
    /// <summary>
    /// flexible Schedule
    /// </summary>
    flexible,
    /// <summary>
    /// remote Schedule
    /// </summary>
    remote,
    /// <summary>
    /// fly in fly out Schedule
    /// </summary>
    flyInFlyOut,
}