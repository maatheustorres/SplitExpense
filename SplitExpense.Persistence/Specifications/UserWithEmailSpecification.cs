using SplitExpense.Domain.Entities;
using SplitExpense.Domain.ValueObjects;
using System.Linq.Expressions;

namespace SplitExpense.Persistence.Specifications;

internal class UserWithEmailSpecification : Specification<User>
{
    private readonly Email _email;

    internal UserWithEmailSpecification(Email email)
      => _email = email;

    internal override Expression<Func<User, bool>> ToExpression() => user => user.Email.Value == _email;
}
