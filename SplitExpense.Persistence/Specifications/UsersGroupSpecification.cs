using SplitExpense.Domain.Entities;
using System.Linq.Expressions;

namespace SplitExpense.Persistence.Specifications;

internal sealed class UsersGroupSpecification : Specification<UserGroup>
{
    private readonly Guid _userId;
    private readonly Guid _groupId;

    public UsersGroupSpecification(UserGroup userGroup)
    {
        _userId = userGroup.UserId;
        _groupId = userGroup.GroupId;
    }

    internal override Expression<Func<UserGroup, bool>> ToExpression() => 
        userGroup => userGroup.UserId == _userId && userGroup.GroupId == _groupId;
}
