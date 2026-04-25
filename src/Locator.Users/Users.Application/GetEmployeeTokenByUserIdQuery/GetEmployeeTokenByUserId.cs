using HeadHunter.Contracts.Dto;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Users.Contracts.Responses;

namespace Users.Application.GetEmployeeTokenByUserIdQuery;

public class GetEmployeeTokenByUserId: IQueryHandler<EmployeeTokenResponse, GetEmployeeTokenByUserIdQuery>
{
    private readonly IUsersReadDbContext _usersDbContext;

    public GetEmployeeTokenByUserId(IUsersReadDbContext usersDbContext)
    {
        _usersDbContext = usersDbContext;
    }

    public async Task<EmployeeTokenResponse> Handle(GetEmployeeTokenByUserIdQuery query, CancellationToken cancellationToken)
    {   
        // Find token in DB
        var tokenRecord = await _usersDbContext.ReadEmployeeTokens
            .Where(u => u.UserId == query.UserId)
            .FirstOrDefaultAsync(cancellationToken);
        if (tokenRecord == null)
        {
            return new EmployeeTokenResponse(null);
        }

        var dto = new EmployeeTokenDto(
            tokenRecord.Id,
            tokenRecord.Token,
            tokenRecord.RefreshToken,
            tokenRecord.CreatedAt,
            tokenRecord.ExpiresAt,
            tokenRecord.UserId);
        
        return new EmployeeTokenResponse(dto);
    } 
}