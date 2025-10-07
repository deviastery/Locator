using Locator.Application.Abstractions;
using Locator.Contracts.Users;

namespace Locator.Application.Users.AuthQuery;

public record AuthQuery(
    AuthorizationCodeDto Dto) : IQuery;