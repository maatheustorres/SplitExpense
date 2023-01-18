using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Group;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Groups.Commands.AddUser;

public sealed class AddUserCommandHandler : IRequestHandler<AddUserCommand, ResultT<AddUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddUserCommandHandler(
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        IUserGroupRepository userGroupRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _userGroupRepository = userGroupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultT<AddUserResponse>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var emails = new List<Email>();

        if (!request.Emails.Any())
        {
            return Result.Failure<AddUserResponse>(DomainErrors.Email.NullOrEmpty);
        }

        foreach (var email in request.Emails)
        {
            ResultT<Email> emailResult = Email.Create(email);

            if (emailResult.IsFailure)
            {
                return Result.Failure<AddUserResponse>(DomainErrors.Email.InvalidFormat);
            }

            emails.Add(emailResult.Value);
        }

        IReadOnlyCollection<User> users = await _userRepository.GetUsersByEmailsAsync(emails);

        if (!users.Any())
        {
            return Result.Failure<AddUserResponse>(DomainErrors.User.NotFound);
        }

        var group = await _groupRepository.GetByIdAsync(request.GroupId);

        if (group is null)
        {
            return Result.Failure<AddUserResponse>(DomainErrors.Group.NotFound);
        }

        var usersGroup = new List<UserGroup>();
        foreach (var user in users)
        {
            var userGroup = new UserGroup()
            {
                UserId = user.Id,
                GroupId = group.Id
            };

            if (await _userGroupRepository.CheckIfAdded(userGroup))
            {
                continue;
            }

            usersGroup.Add(userGroup);
        }

        if (!usersGroup.Any())
        {
            return Result.Failure<AddUserResponse>(DomainErrors.Group.AlreadyAdded);
        }

        _userGroupRepository.InsertRange(usersGroup);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AddUserResponse(users.Select(x => x.Email.Value).ToList()));
    }
}
