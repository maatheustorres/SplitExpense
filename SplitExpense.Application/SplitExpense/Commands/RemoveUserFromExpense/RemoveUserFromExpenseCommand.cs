using MediatR;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.SplitExpense.Commands.RemoveUserFromExpense;

public sealed record RemoveUserFromExpenseCommand(Guid Id) : IRequest<Result>;