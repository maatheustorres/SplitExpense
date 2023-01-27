using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Expenses.Commands.AddUserToExpense;

public sealed class AddUserToExpenseCommandHandler : IRequestHandler<AddUserToExpenseCommand, Result>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IExpenseUserRepository _expenseUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddUserToExpenseCommandHandler(
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

    public async Task<Result> Handle(AddUserToExpenseCommand request, CancellationToken cancellationToken)
    {
        if (request.UserIds.Count == 0)
        {
            return Result.Failure(DomainErrors.User.NullOrEmpty);
        }

        ResultT<Group> groupResult = await _groupRepository.GetByIdAsync(request.GroupId);

        if(groupResult.Value is null)
        {
            return Result.Failure(DomainErrors.Group.NotFound);
        }

        Group group = groupResult.Value;

        ResultT<Expense> expenseResult = await _expenseRepository.GetByIdAsync(request.ExpenseId);

        if (expenseResult.Value is null)
        {
            return Result.Failure(DomainErrors.Expense.NotFound);
        }

        Expense expense = expenseResult.Value;

        if (request.Pay < 1)
        {
            return Result.Failure(DomainErrors.Expense.InvalidExpense);
        }

        var expenseUsers = new List<ExpenseUsers>();
        foreach (var userIds in request.UserIds)
        {
            ResultT<User> userResult = await _userRepository.GetByIdAsync(userIds);

            if (userResult.Value is null)
            {
                continue;
            }

            User user = userResult.Value;

            UserGroup userGroup = Group.AddUserToGroup(user, group);

            if (!await _userGroupRepository.CheckIfAddedToGroup(userGroup))
            {
                continue;
            }

            var expenseUser = expense.AddUsersToExpense(request.Pay, user, expense);

            if (!await _expenseUserRepository.CheckIfAddedToExpense(expenseUser))
            {
                continue;
            }

            expenseUsers.Add(expenseUser);
        }

        if (!expenseUsers.Any())
        {
            return Result.Failure(DomainErrors.Expense.AlreadyAdded);
        }

        _expenseUserRepository.InsertRange(expenseUsers);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
