using CSharpFunctionalExtensions;
using FluentValidation;
using Framework.Extensions;
using HeadHunter.Contracts;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Abstractions;
using Shared.Thesauruses;
using Vacancies.Application.Fails;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;
using Vacancies.Domain;

namespace Vacancies.Application.CreateNegotiationCommand;

    public class CreateNegotiationCommandHandler : ICommandHandler<CreateNegotiationCommand>
{
    private readonly IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> 
        _getRequestEmployeeTokenQuery;
    private readonly IVacanciesContract _vacanciesContract;
    private readonly ILogger<CreateNegotiationCommandHandler> _logger;
    
    public CreateNegotiationCommandHandler(
        IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> getRequestEmployeeTokenQuery,
        IVacanciesContract vacanciesContract, 
        ILogger<CreateNegotiationCommandHandler> logger)
    {
        _getRequestEmployeeTokenQuery = getRequestEmployeeTokenQuery;
        _vacanciesContract = vacanciesContract;
        _logger = logger;
    }
    public async Task<UnitResult<Failure>> Handle(
        CreateNegotiationCommand command,
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        var getRequestEmployeeTokenQuery = new GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery(
            command.UserId);
        var employeeTokenResponse = await _getRequestEmployeeTokenQuery.Handle(
            getRequestEmployeeTokenQuery,
            cancellationToken);
        if (employeeTokenResponse.EmployeeToken?.Token == null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }
        
        string token = employeeTokenResponse.EmployeeToken.Token;
        
        if (token is null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        // Get all resume IDs of user
        var resumesResult = await _vacanciesContract
            .GetResumeIdsAsync(token, cancellationToken);
        if (resumesResult.IsFailure && resumesResult.Error.Code == "record.not.found")
        {
            throw new GetValidResumeNotFoundException("Resumes not found");
        }
        if (resumesResult.IsFailure)
        {
            throw new GetResumeFailureException();
        }
        
        var resume = resumesResult.Value?.Resumes?
            .FirstOrDefault(r => r.Status?.Id == nameof(ResumeStatusEnum.PUBLISHED).ToLower());
        if (resume is null)
        {
            throw new GetValidResumeNotFoundException("Resumes not found");
        }
        
        if (!long.TryParse(resume.Id, out long resumeId))
        {
            return Errors.TryParseResumeIdFail().ToFailure();
        }
        
        // Create a negotiation to a Vacancy by ID
        var createNegotiationResult = await _vacanciesContract
            .CreateNegotiationByVacancyIdAsync(
                command.VacancyId, 
                resumeId,
                token, 
                cancellationToken);
        if (createNegotiationResult.IsFailure)
        {
            _logger.LogError(
                "Failed to create negotiation for vacancy {VacancyId}, resume {ResumeId}, user {UserId}. Error: {ErrorCode} - {ErrorMessage}",
                command.VacancyId, 
                resumeId, 
                command.UserId,
                createNegotiationResult.Error.Code, 
                createNegotiationResult.Error.Message);
            
            return createNegotiationResult.Error.ToFailure();
        }
        
        _logger.LogInformation(
            "Successfully created negotiation for vacancy {VacancyId} and user {UserId}",
            command.VacancyId, 
            command.UserId);
        return UnitResult.Success<Failure>();
    }
}