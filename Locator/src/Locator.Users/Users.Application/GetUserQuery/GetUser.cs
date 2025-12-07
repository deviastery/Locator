using HeadHunter.Contracts.Dto;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Users.Contracts.Dto;
using Users.Contracts.Responses;

namespace Users.Application.GetUserQuery;

public class GetUser: IQueryHandler<UserResponse, GetUserQuery>
{
    private readonly IUsersReadDbContext _usersDbContext;

    public GetUser(IUsersReadDbContext usersDbContext)
    {
        _usersDbContext = usersDbContext;
    }

    public async Task<UserResponse> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {   
        // Find user in DB
        var userRecord = await _usersDbContext.ReadUsers
            .Where(u => u.Id == query.Dto.UserId)
            .FirstOrDefaultAsync(cancellationToken);
        if (userRecord == null)
        {
            return new UserResponse(null);
        }

        var dto = new UserDto(userRecord.EmployeeId.ToString(), userRecord.Name, userRecord.Email);
        
        return new UserResponse(dto);
    } 
}