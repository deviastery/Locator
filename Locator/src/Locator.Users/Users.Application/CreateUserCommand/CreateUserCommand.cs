using Shared.Abstractions;
using Users.Contracts.Dto;

namespace Users.Application.CreateUserCommand;

public record CreateUserCommand(Guid UserId, CreateUserDto Dto) : ICommand;