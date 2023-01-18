using MediatR;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Groups.Commands.UpdateGroupName;

public sealed class UpdateGroupNameCommandHandler : IRequestHandler<UpdateGroupNameCommand, Result>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGroupNameCommandHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateGroupNameCommand request, CancellationToken cancellationToken)
    {
        ResultT<Group> groupById = await _groupRepository.GetByIdAsync(request.GroupId);

        if(groupById.Value is null)
        {
            return Result.Failure(DomainErrors.Group.NotFound);
        }

        Group group = groupById.Value;

        ResultT<Name> nameResult = Name.Create(request.Name);

        if (nameResult.IsFailure)
        {
            return Result.Failure(nameResult.Error);
        }

        group.ChangeName(nameResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
