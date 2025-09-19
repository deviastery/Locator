using System.Text.Json;
using Shared;

namespace Locator.Application.Exceptions;

public class BadRequestException : Exception
{
    protected BadRequestException(IEnumerable<Error> errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }
}