using Shared.Abstractions;

namespace Users.Application.GetUserQuery;

public record GetUserQuery(Guid UserId) : IQuery;