using MediatR;
using Microsoft.EntityFrameworkCore;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Users;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;

namespace SplitExpense.Application.Users.Queries.GetUsersByGroupId;

public sealed class GetUsersByGroupIdQueryHandler : IRequestHandler<GetUsersByGroupIdQuery, ResultT<IReadOnlyCollection<UserResponse>>>
{
    private readonly IDbContext _dbContext;

    public GetUsersByGroupIdQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResultT<IReadOnlyCollection<UserResponse>>> Handle(GetUsersByGroupIdQuery request, CancellationToken cancellationToken)
    {
        if(request.GroupId == Guid.Empty)
        {
            return Result.Failure<IReadOnlyCollection<UserResponse>>(Error.NullValue);
        }

        ResultT<IReadOnlyCollection<UserResponse>> users = await (
            from user in _dbContext.Set<User>().AsNoTracking()
            join userGroup in _dbContext.Set<UserGroup>().AsNoTracking()
                on user.Id equals userGroup.UserId
            where userGroup.GroupId == request.GroupId
            select new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FistName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
            }).ToListAsync();

        if(!users.Value.Any())
        {
            return Result.Failure<IReadOnlyCollection<UserResponse>>(DomainErrors.Group.NoUser);
        }

        return users;
    }
}
