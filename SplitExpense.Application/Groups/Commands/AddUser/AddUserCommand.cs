using MediatR;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Contracts.Group;

namespace SplitExpense.Application.Groups.Commands.AddUser;

public sealed record AddUserCommand(
    Guid GroupId, 
    IReadOnlyList<string> Emails) : IRequest<ResultT<AddUserResponse>>;
