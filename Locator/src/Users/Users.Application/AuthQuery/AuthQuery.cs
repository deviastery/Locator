using Shared.Abstractions;
using Users.Contracts.Dto;

namespace Users.Application.AuthQuery;

public record AuthQuery(AuthorizationCodeDto Dto) : IQuery;