using System.Text.Json;

namespace Shared.Fails.Exceptions;

public class FailureException : Exception
{
    protected FailureException(IEnumerable<Error> errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }
}