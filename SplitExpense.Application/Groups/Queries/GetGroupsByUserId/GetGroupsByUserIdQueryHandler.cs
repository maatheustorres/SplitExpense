using MediatR;
using Microsoft.EntityFrameworkCore;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Common;
using SplitExpense.Contracts.Group;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Enumerations;
using SplitExpense.Domain.Repositories;

namespace SplitExpense.Application.Groups.Queries.GetGroupsByUserId;

public sealed class GetGroupsByUserIdQueryHandler : IRequestHandler<GetGroupsByUserIdQuery, ResultT<PagedList<GroupResponse>>>
{
    private readonly IDbContext _dbContext;
    private readonly IUserRepository _userRepository;

    public GetGroupsByUserIdQueryHandler(IDbContext dbContext, IUserRepository userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    public async Task<ResultT<PagedList<GroupResponse>>> Handle(GetGroupsByUserIdQuery request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId) is null)
        {
            return Result.Failure<PagedList<GroupResponse>>(DomainErrors.User.NotFound);
        }

        IQueryable<GroupResponse> groupResponsesQuery =
            from groupName in _dbContext.Set<Group>().AsNoTracking()
            join userGroup in _dbContext.Set<UserGroup>().AsNoTracking()
                on groupName.Id equals userGroup.GroupId
            join user in _dbContext.Set<User>().AsNoTracking()
                on userGroup.UserId equals user.Id
            where userGroup.UserId == request.UserId
            orderby groupName.CreatedOnUtc
            select new GroupResponse
            {
                Id = groupName.Id,
                Name = groupName.Name,
                CategoryId = groupName.Category.Value,
                CreatedOnUtc = groupName.CreatedOnUtc,
                UserGroupId = userGroup.Id,
                UserId = user.Id,
                fullname = $"{user.FirstName} {user.LastName}"
            };

        int totalCount = await groupResponsesQuery.CountAsync(cancellationToken);

        GroupResponse[] groupResponsesPage = await groupResponsesQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken);

        foreach (var groupResponse in groupResponsesPage)
        {
            groupResponse.Category = Category.FromValue(groupResponse.CategoryId).Name;
        }

        return new PagedList<GroupResponse>(groupResponsesPage, request.Page, request.PageSize, totalCount);
    }
}
