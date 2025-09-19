using Locator.Application.Exceptions;
using Shared;

namespace Locator.Application.Ratings.Fails.Exceptions;


public class RatingValidationException : BadRequestException
{
    public RatingValidationException(IEnumerable<Error> errors) 
        : base(errors)
    {
    }
}