using Locator.Application.Abstractions;
using Locator.Contracts.Users;
using Locator.Contracts.Users.Dtos;

namespace Locator.Application.Users.AuthQuery;

public record AuthQuery(
    AuthorizationCodeDto Dto) : IQuery;