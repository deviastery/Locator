using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class TooEarlyReviewException : BadRequestException
{
    public TooEarlyReviewException() 
        : base([Errors.Vacancies.TooEarleReview()])
    {
    }
}