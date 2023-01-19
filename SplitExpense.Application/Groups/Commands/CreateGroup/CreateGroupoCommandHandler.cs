using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Group;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Enumerations;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Groups.Commands.Create;

public sealed class CreateGroupoCommandHandler : IRequestHandler<CreateGroupCommand, ResultT<GroupResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGroupRepository _groupRepository;

    public CreateGroupoCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _groupRepository = groupRepository;
    }

    public async Task<ResultT<GroupResponse>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        User user = await _userRepository.GetByIdAsync(request.UserId);

        if(user is null)
        {
            return Result.Failure<GroupResponse>(DomainErrors.User.NotFound);
        }

        Category category = Category.FromValue(request.CategoryId);

        if (category is null)
        {
            return Result.Failure<GroupResponse>(DomainErrors.Category.NotFound);
        }

        ResultT<Name> nameResult = Name.Create(request.Name);

        if (nameResult.IsFailure)
        {
            return Result.Failure<GroupResponse>(nameResult.Error);
        }

        var group = Group.Create(nameResult.Value, category);

        _groupRepository.Insert(group);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new GroupResponse
        {
            Id = group.Id,
            Name = group.Name.Value,
            CategoryId = group.Category.Value,
            Category = group.Category.Name,
            CreatedOnUtc = group.CreatedOnUtc
        });
    }
}
