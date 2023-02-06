using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Expense;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Expenses.Commands.UpdateExpense;

public sealed class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, ResultT<ExpensesResponse>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserGroupRepository _userGroupRepository;

    public UpdateExpenseCommandHandler(IExpenseRepository expenseRepository, IUnitOfWork unitOfWork, IUserGroupRepository userGroupRepository)
    {
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
        _userGroupRepository = userGroupRepository;
    }

    public async Task<ResultT<ExpensesResponse>> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        ResultT<Expense> expenseResult = await _expenseRepository.GetByIdAsync(request.Id);

        if (expenseResult.Value is null)
        {
            return Result.Failure<ExpensesResponse>(DomainErrors.Expense.NotFound);
        }

        Expense expense = expenseResult.Value;

        ResultT<UserGroup> userGroupByUserGroupId = await _userGroupRepository.GetByIdAsync(expense.UserGroupId);

        if (userGroupByUserGroupId.Value is null)
        {
            return Result.Failure<ExpensesResponse>(Error.NullValue);
        }

        UserGroup validUserGroup = userGroupByUserGroupId.Value;

        if (validUserGroup.Id != expense.UserGroupId)
        {
            return Result.Failure<ExpensesResponse>(DomainErrors.User.InvalidPermissions);
        }

        ResultT<TotalExpense> totalExpenseResult = TotalExpense.Create(request.Expense);

        if (totalExpenseResult.IsFailure)
        {
            return Result.Failure<ExpensesResponse>(totalExpenseResult.Error);
        }

        Result result = expense.Update(totalExpenseResult.Value, request.Paid);

        if (result.IsFailure)
        {
            return Result.Failure<ExpensesResponse>(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ExpensesResponse()
        {
            TotalExpense = expense.TotalExpense.Value,
            Paid = expense.Paid,
            UserGroupId = expense.UserGroupId,
            ExpenseId = expense.Id
        });
    }
}
