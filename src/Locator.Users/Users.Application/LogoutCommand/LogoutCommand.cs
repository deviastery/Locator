using Shared.Abstractions;

namespace Users.Application.LogoutCommand;

public record LogoutCommand(Guid UserId) : ICommand;