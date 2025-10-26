using System.Text.Json;
using Shared;

namespace Locator.Infrastructure.HhApi.Exceptions;

public class BadRequestException : Exception
{
    protected BadRequestException(IEnumerable<Error> errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }
}