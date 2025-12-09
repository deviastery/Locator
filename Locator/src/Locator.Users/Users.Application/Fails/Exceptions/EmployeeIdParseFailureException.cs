using Shared.Fails.Exceptions;

namespace Users.Application.Fails.Exceptions;

public class EmployeeIdParseFailureException : FailureException
{
    public EmployeeIdParseFailureException() 
        : base([Errors.General.Failure("Parsing error employee Id.")])
    {
    }
}