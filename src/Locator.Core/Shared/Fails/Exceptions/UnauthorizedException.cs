using System.Text.Json;

namespace Shared.Fails.Exceptions;

public class UnauthorizedException : Exception
{
    protected UnauthorizedException(IEnumerable<Error> errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }
}