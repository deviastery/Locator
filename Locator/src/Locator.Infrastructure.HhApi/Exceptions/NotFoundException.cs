using System.Text.Json;
using Shared;

namespace Locator.Infrastructure.HhApi.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(IEnumerable<Error> errors) 
        : base(JsonSerializer.Serialize(errors))
    {
    }
}

