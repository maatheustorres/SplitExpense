using MediatR;
using SplitExpense.Contracts.Group;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Groups.Queries.GetUsersByGroupId;

public sealed record GetUsersByGroupIdQuery(Guid GroupId) : IRequest<ResultT<List<UsersByGroupResponse>>>;
