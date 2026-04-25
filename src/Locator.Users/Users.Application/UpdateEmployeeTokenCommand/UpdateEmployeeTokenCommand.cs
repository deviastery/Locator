using HeadHunter.Contracts.Dto;
using Shared.Abstractions;
using Users.Contracts.Dto;

namespace Users.Application.UpdateEmployeeTokenCommand;

public record UpdateEmployeeTokenCommand(EmployeeTokenDto Token) : ICommand;