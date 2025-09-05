using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetUsersRespondedToVacancyById(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Users get");
    }
}