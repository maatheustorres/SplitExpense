using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Enumerations;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Group.Commands.Create;

public sealed class CreateGroupoCommandHandler : IRequestHandler<CreateGroupCommand, Result>
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

    public async Task<Result> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        User user = await _userRepository.GetByIdAsync(request.UserId);

        if(user is null)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        Category category = Category.FromValue(request.CategoryId);

        if (category is null)
        {
            return Result.Failure(DomainErrors.Category.NotFound);
        }

        ResultT<Name> nameResult = Name.Create(request.Name);

        if (nameResult.IsFailure)
        {
            return Result.Failure(nameResult.Error);
        }

        var group = Domain.Entities.Group.Create(nameResult.Value, category);

        _groupRepository.Insert(group);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
