using MediatR;
using SplitExpense.Contracts.Users;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<ResultT<UserResponse>>;
