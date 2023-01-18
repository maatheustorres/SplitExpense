using MediatR;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Groups.Commands.UpdateGroupName;

public sealed record UpdateGroupNameCommand(Guid GroupId, string Name) : IRequest<Result>;
