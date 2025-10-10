using Locator.Application.Abstractions;

namespace Locator.Application.Users.RefreshTokenQuery;

public record RefreshTokenQuery(string RefreshToken) : IQuery;