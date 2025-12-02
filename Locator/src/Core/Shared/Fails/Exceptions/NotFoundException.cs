using System.Text.Json;

namespace Shared.Fails.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(IEnumerable<Error> errors) 
        : base(JsonSerializer.Serialize(errors))
    {
    }
}