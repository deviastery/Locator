using System.Text.Json;
using Shared;

namespace Locator.Application.Exceptions;

public class UnauthorizedException : Exception
{
    protected UnauthorizedException(IEnumerable<Error> errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }
}