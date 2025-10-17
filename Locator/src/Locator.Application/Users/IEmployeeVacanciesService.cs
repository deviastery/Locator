using CSharpFunctionalExtensions;
using Locator.Contracts.Users;
using Locator.Contracts.Vacancies;
using Shared;

namespace Locator.Application.Users;

public interface IEmployeeVacanciesService
{
    Task<Result<ResumesResponse, Error>> GetResumeIdsAsync(
        string accessToken, 
        CancellationToken cancellationToken);
    Task<Result<EmployeeVacanciesResponse, Error>> GetVacanciesMatchResumeAsync(
        string resumeId, 
        string accessToken, 
        CancellationToken cancellationToken);
    Task<Result<VacancyDto, Error>> GetVacancyByIdAsync(
        string vacancyId,
        string accessToken,
        CancellationToken cancellationToken);
}