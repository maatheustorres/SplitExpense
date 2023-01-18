using MediatR;
using SplitExpense.Contracts.Users;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Users.Queries.GetUsersByGroupId;

public sealed record GetUsersByGroupIdQuery(Guid GroupId) 
    : IRequest<ResultT<IReadOnlyCollection<UserResponse>>>;
