using Locator.Application.Abstractions;

namespace Locator.Application.Users.RefreshTokenCommand;

public record RefreshTokenCommand(Guid UserId) : ICommand;