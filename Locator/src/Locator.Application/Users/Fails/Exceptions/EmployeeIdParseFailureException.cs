using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class EmployeeIdParseFailureException : FailureException
{
    public EmployeeIdParseFailureException() 
        : base([Errors.General.Failure("Parsing error employee Id.")])
    {
    }
}