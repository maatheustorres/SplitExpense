using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Expenses.Commands.CreateExpense;

public sealed class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Result>
{
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExpenseCommandHandler(
        IUserGroupRepository userGroupRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        IExpenseRepository expenseRepository)
    {
        _userGroupRepository = userGroupRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<Result> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        ResultT<User> userResult = await _userRepository.GetByIdAsync(request.UserId);

        if (userResult.Value is null)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        User user = userResult.Value;

        ResultT<Group> groupResult = await _groupRepository.GetByIdAsync(request.GroupId);

        if (groupResult.Value is null)
        {
            return Result.Failure(DomainErrors.Group.NotFound);
        }

        Group group = groupResult.Value;

        UserGroup userGroup = Group.AddUserToGroup(user, group);

        if (!await _userGroupRepository.CheckIfAdded(userGroup))
        {
            return Result.Failure(DomainErrors.User.InvalidPermissions);
        }

        ResultT<TotalExpense> totalExpenseResult = TotalExpense.Create(request.TotalExpense);

        if(totalExpenseResult.IsFailure)
        {
            return Result.Failure(totalExpenseResult.Error);
        }

        var expense = Expense.Create(totalExpenseResult.Value, request.Paid, request.UserGroupId);

        _expenseRepository.Insert(expense);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
