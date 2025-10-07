using System.Text.Json;
using Shared;

namespace Locator.Application.Exceptions;

public class FailureException : Exception
{
    protected FailureException(IEnumerable<Error> errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }
}