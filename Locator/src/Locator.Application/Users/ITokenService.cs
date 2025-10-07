namespace Locator.Application.Users;

public interface ITokenService
{
    string GenerateToken(Guid userId, string email);
}