using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;

namespace SplitExpense.Application.SplitExpense.Commands.CreateSplitExpense;

public sealed class CreateSplitExpenseCommandHandler : IRequestHandler<CreateSplitExpenseCommand, Result>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IExpenseUserRepository _expenseUserRepository;
    private readonly IUnitOfWork _unitOfWork;
    private const int resposibleUserByExpense = 1;

    public CreateSplitExpenseCommandHandler(
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

    public async Task<Result> Handle(CreateSplitExpenseCommand request, CancellationToken cancellationToken)
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

        ResultT<Group> groupResult = await _groupRepository.GetByIdAsync(validUserGroup.GroupId);

        if (groupResult.Value is null)
        {
            return Result.Failure(DomainErrors.Group.NotFound);
        }

        Group group = groupResult.Value;

        if (request.UserIds.Count == 0)
        {
            return Result.Failure(DomainErrors.User.NullOrEmpty);
        }

        var usersToPay = request.UserIds.Count;

        decimal splitValueToPay = expense.TotalExpense.Value / (usersToPay + resposibleUserByExpense);

        var expenseUsers = new List<ExpenseUsers>();
        foreach (var userId in request.UserIds)
        {
            ResultT<User> userResult = await _userRepository.GetByIdAsync(userId);

            if (userResult.Value is null)
            {
                return Result.Failure(DomainErrors.User.NotFound);
            }

            User user = userResult.Value;

            UserGroup userGroup = Group.AddUserToGroup(user, group);

            if (!await _userGroupRepository.CheckIfAddedToGroup(userGroup))
            {
                return Result.Failure(DomainErrors.Group.AlreadyAdded);
            }

            var expenseUser = expense.AddUsersToExpense(splitValueToPay, user, expense);

            if (await _expenseUserRepository.CheckIfAddedToExpense(expenseUser))
            {
                return Result.Failure(DomainErrors.Expense.AlreadyAdded);
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
