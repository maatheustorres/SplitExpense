using MediatR;
using SplitExpense.Contracts.Group;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Groups.Commands.Create;

public sealed record CreateGroupCommand(
    Guid UserId,
    string Name, 
    int CategoryId) : IRequest<ResultT<GroupResponse>>;
