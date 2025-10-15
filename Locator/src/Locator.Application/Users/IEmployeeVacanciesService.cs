using CSharpFunctionalExtensions;
using Locator.Contracts.Users;
using Locator.Contracts.Vacancies;
using Shared;

namespace Locator.Application.Users;

public interface IEmployeeVacanciesService
{
    Task<Result<ResumesResponse, Error>> GetUserResumeIdsAsync(
        string accessToken, 
        CancellationToken cancellationToken);
    Task<Result<EmployeeVacanciesResponse, Error>> GetUserVacanciesAsync(
        string resumeId, 
        string accessToken, 
        CancellationToken cancellationToken);
}