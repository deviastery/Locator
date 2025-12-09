using Shared.Abstractions;
using Vacancies.Contracts.Dto;

namespace Users.Application.CreateUserCommand;

public record CreateUserCommand(CreateUserDto Dto) : ICommand;