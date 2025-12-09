using System.Text.Json;

namespace Shared.Fails.Exceptions;

public class BadRequestException : Exception
{
    protected BadRequestException(IEnumerable<Error> errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }
}