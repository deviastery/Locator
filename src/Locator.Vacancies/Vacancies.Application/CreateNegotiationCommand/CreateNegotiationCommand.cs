using Shared.Abstractions;
using Vacancies.Contracts.Dto;

namespace Vacancies.Application.CreateNegotiationCommand;

public record CreateNegotiationCommand(long VacancyId, Guid UserId) : ICommand;