using Shared.Abstractions;

namespace Vacancies.Application.GetRequestEmployeeTokenQuery;

public record GetRequestEmployeeTokenQuery(Guid UserId) : IQuery;