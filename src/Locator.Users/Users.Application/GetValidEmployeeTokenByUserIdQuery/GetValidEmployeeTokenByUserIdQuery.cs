using Shared.Abstractions;

namespace Users.Application.GetValidEmployeeTokenByUserIdQuery;

public record GetValidEmployeeTokenByUserIdQuery(Guid UserId) : IQuery;