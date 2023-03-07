using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;

namespace SplitExpense.Application.SplitExpense.Commands.UpdateSplitExpense;

public sealed class UpdateSplitExpenseCommandHandler : IRequestHandler<UpdateSplitExpenseCommand, Result>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IExpenseUserRepository _expenseUserRepository;
    private readonly IUnitOfWork _unitOfWork;
    private const int resposibleUserByExpense = 1;

    public UpdateSplitExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        IUserGroupRepository userGroupRepository,
        IExpenseUserRepository expenseUserRepository,
        IUnitOfWork unitOfWork)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _userGroupRepository = userGroupRepository;
        _expenseUserRepository = expenseUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateSplitExpenseCommand request, CancellationToken cancellationToken)
    {
        ResultT<Expense> expenseResult = await _expenseRepository.GetByIdAsync(request.ExpenseId);

        if (expenseResult.Value is null)
        {
            return Result.Failure(DomainErrors.Expense.NotFound);
        }

        Expense expense = expenseResult.Value;

        if (expense.Paid)
        {
            return Result.Failure(DomainErrors.Expense.AlreadyPaid);
        }

        ResultT<UserGroup> userGroupByUserGroupId = await _userGroupRepository.GetByIdAsync(expense.UserGroupId);

        if (userGroupByUserGroupId.Value is null)
        {
            return Result.Failure(Error.NullValue);
        }

        UserGroup validUserGroup = userGroupByUserGroupId.Value;

        if (validUserGroup.UserId != request.UserId)
        {
            return Result.Failure(DomainErrors.User.InvalidPermissions);
        }

        if (request.UserIds.Count == 0)
        {
            return Result.Failure(DomainErrors.User.NullOrEmpty);
        }

        var usersToPay = request.UserIds.Count;

        decimal splitValueToPay = expense.TotalExpense.Value / (usersToPay + resposibleUserByExpense);

        List<ExpenseUsers> expenseUser = await _expenseUserRepository.GetExpenseUserByExpenseId(expense.Id);

        if (expenseUser.Any())
        {
            foreach (var item in expenseUser)
            {
                _expenseUserRepository.Remove(item);
            }

            foreach (var userIds in request.UserIds)
            {
                ResultT<User> userResult = await _userRepository.GetByIdAsync(userIds);

                if (userResult.Value is null)
                {
                    return Result.Failure(DomainErrors.User.NotFound);
                }

                User user = userResult.Value;

                ExpenseUsers expenseUsers = expense.AddUsersToExpense(splitValueToPay, user, expense);

                _expenseUserRepository.Insert(expenseUsers);

            }
        }
        else
        {
            return Result.Failure(DomainErrors.ExpenseUser.NoUser);
        }


        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
