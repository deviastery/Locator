namespace Shared.Fails.Exceptions;

public class ConfigurationFailureException : FailureException
{
    public ConfigurationFailureException(string message)
        : base([Error.Failure(message, "configuration.failure")])
    {
    }
}