using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;

namespace SplitExpense.Application.SplitExpense.Commands.RemoveUserFromExpense;

public sealed class RemoveUserFromExpenseCommandHandler : IRequestHandler<RemoveUserFromExpenseCommand, Result>
{
    private readonly IExpenseUserRepository _expenseUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveUserFromExpenseCommandHandler(IExpenseUserRepository expenseUserRepository, IUnitOfWork unitOfWork)
    {
        _expenseUserRepository = expenseUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveUserFromExpenseCommand request, CancellationToken cancellationToken)
    {
        ResultT<ExpenseUsers> expenseUsersResult = await _expenseUserRepository.GetByIdAsync(request.Id);

        if(expenseUsersResult.Value is null)
        {
            return Result.Failure(DomainErrors.Expense.UserNotAdded);
        }

        ExpenseUsers expenseUsers = expenseUsersResult.Value;

        _expenseUserRepository.Remove(expenseUsers);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
