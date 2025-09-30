using CSharpFunctionalExtensions;
using Shared;

namespace Locator.Application.Abstractions;

public interface ICommand;
public interface IHandler<TResponse, in TCommand>
where TCommand : ICommand
{
    Task<Result<TResponse, Failure>> Handle(TCommand command, CancellationToken cancellationToken);
}
public interface IHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<Failure>> Handle(TCommand command, CancellationToken cancellationToken);
}