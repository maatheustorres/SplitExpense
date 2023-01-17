using MediatR;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Group.Commands.Create;

public sealed record CreateGroupCommand(
    Guid UserId,
    string Name, 
    int CategoryId) : IRequest<Result>;
