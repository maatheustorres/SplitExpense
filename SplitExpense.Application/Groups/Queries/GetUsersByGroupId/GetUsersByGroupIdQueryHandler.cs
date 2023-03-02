using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Group;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SplitExpense.Application.Groups.Queries.GetUsersByGroupId;

public sealed class GetUsersByGroupIdQueryHandler : IRequestHandler<GetUsersByGroupIdQuery, ResultT<List<UsersByGroupResponse>>>
{
    private readonly IDbContext _dbContext;
    private readonly IGroupRepository _groupRepository;

    public GetUsersByGroupIdQueryHandler(IDbContext dbContext, IGroupRepository groupRepository)
    {
        _dbContext = dbContext;
        _groupRepository = groupRepository;
    }

    public async Task<ResultT<List<UsersByGroupResponse>>> Handle(GetUsersByGroupIdQuery request, CancellationToken cancellationToken)
    {
        if (await _groupRepository.GetByIdAsync(request.GroupId) is null)
        {
            return Result.Failure<List<UsersByGroupResponse>>(DomainErrors.Group.NotFound);
        }

        List<UsersByGroupResponse> users = await (
            from usergroup in _dbContext.Set<UserGroup>().AsNoTracking()
            join user in _dbContext.Set<User>().AsNoTracking()
                on usergroup.UserId equals user.Id
            where usergroup.GroupId == request.GroupId
            select new UsersByGroupResponse
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}"
            }).ToListAsync();

        return Result.Success(users);
    }
}
