using MediatR;
using SplitExpense.Contracts.Common;
using SplitExpense.Contracts.Group;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Groups.Queries.GetGroupsByUserId;

public sealed record GetGroupsByUserIdQuery(
    Guid UserId,
    int Page,
    int PageSize) : IRequest<ResultT<PagedList<GroupResponse>>>;
