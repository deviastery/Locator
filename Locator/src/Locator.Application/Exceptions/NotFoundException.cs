using System.Text.Json;
using Shared;

namespace Locator.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(IEnumerable<Error> errors) 
        : base(JsonSerializer.Serialize(errors))
    {
    }
}

