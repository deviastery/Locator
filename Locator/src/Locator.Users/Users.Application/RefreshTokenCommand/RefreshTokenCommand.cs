using Shared.Abstractions;

namespace Users.Application.RefreshTokenCommand;

public record RefreshTokenCommand(Guid UserId) : ICommand;