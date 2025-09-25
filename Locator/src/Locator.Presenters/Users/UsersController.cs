using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters.Users;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    public UsersController()
    {
    }
    
    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetUsersRespondedToVacancyById(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Users get");
    }
}