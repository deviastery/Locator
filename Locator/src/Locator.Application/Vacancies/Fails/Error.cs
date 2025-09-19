using Shared;

namespace Locator.Application.Vacancies.Fails;

public partial class Errors
{
    public static class Vacancies
    {
        public static Error TooEarleReview() =>
            Error.Failure(
                "review.too.early",
                "Можно оставить отзыв только спустя 5 дней после отклика.");
    }
}