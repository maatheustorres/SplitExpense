using SplitExpense.Domain.Entities;
using SplitExpense.Domain.ValueObjects;
using System.Linq.Expressions;

namespace SplitExpense.Persistence.Specifications;

internal sealed class UsersGroupWithEmailSpecification : Specification<User>
{
    private readonly string[] _emails;

    internal UsersGroupWithEmailSpecification(IReadOnlyCollection<Email> emails) =>
        _emails = emails.Select(email => email.Value).Distinct().ToArray();

    internal override Expression<Func<User, bool>> ToExpression() => user => _emails.Contains(user.Email.Value);
}
