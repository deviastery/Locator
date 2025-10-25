using System.Text.Json;
using Shared;

namespace Locator.Infrastructure.HhApi.Exceptions;

public class UnauthorizedException : Exception
{
    protected UnauthorizedException(IEnumerable<Error> errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }
}