using Shared.Abstractions;

namespace Users.Application.GetEmployeeTokenByUserIdQuery;

public record GetEmployeeTokenByUserIdQuery(Guid UserId) : IQuery;