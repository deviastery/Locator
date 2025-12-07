using Shared.Abstractions;
using Users.Contracts.Dto;

namespace Users.Application.GetUserQuery;

public record GetUserQuery(Guid UserId) : IQuery;