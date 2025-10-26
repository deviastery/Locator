﻿using CSharpFunctionalExtensions;
using Locator.Contracts.Users.Responses;
using Locator.Contracts.Vacancies.Dto;
using Locator.Contracts.Vacancies.Responses;
using Shared;

namespace Locator.Application.Users;

public interface IVacanciesService
{
    /// <summary>
    /// Gets IDs of user resumes 
    /// </summary>
    /// <param name="accessToken">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of resumes with pagination info</returns>
    Task<Result<ResumesResponse, Error>> GetResumeIdsAsync(
        string accessToken, 
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets vacancies that match resume
    /// </summary>
    /// <param name="resumeId">ID of resume</param>
    /// <param name="query">Filters and pagination</param>
    /// <param name="accessToken">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of vacancies with pagination info</returns>
    Task<Result<EmployeeVacanciesResponse, Error>> GetVacanciesMatchResumeAsync(
        string resumeId, 
        GetVacanciesDto query,
        string accessToken, 
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets vacancy by ID
    /// </summary>
    /// <param name="vacancyId">Vacancy ID</param>
    /// <param name="accessToken">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Vacancy</returns>
    Task<Result<VacancyDto, Error>> GetVacancyByIdAsync(
        string vacancyId,
        string accessToken,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets negotiations of user
    /// </summary>
    /// <param name="query">Pagination</param>
    /// <param name="accessToken">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of negotiations with pagination info</returns>
    Task<Result<EmployeeNegotiationsResponse, Error>> GetNegotiationsByUserIdAsync(
        GetNegotiationsDto query,
        string accessToken,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets negotiation by vacancy ID
    /// </summary>
    /// <param name="vacancyId">Vacancy ID</param>
    /// <param name="accessToken">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Negotiation</returns>
    Task<Result<NegotiationDto, Error>> GetNegotiationByVacancyIdAsync(
        long vacancyId, 
        string accessToken, 
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets negotiation by ID
    /// </summary>
    /// <param name="negotiationId">ID of negotiation</param>
    /// <param name="accessToken">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Negotiation</returns>
    Task<Result<NegotiationDto, Error>> GetNegotiationByIdAsync(
        long negotiationId,
        string accessToken,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets the difference between today and the response day in days
    /// </summary>
    /// <param name="negotiationId">Negotiation ID</param>
    /// <param name="accessToken">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of days</returns>
    Task<Result<int, Error>> GetDaysAfterApplyingAsync(
        long negotiationId, 
        string accessToken, 
        CancellationToken cancellationToken);
}