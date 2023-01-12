using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Domain.Entities;
public sealed class User : AggregateRoot
{
    private string _passwordHash;

    public User(FirstName firstName, LastName lastName, Email email, string passwordHash)
        : base(Guid.NewGuid())
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        _passwordHash = passwordHash;
    }

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }

    public string FullName => $"{FirstName} {LastName}";
    public Email Email { get; private set; }
    public DateTime CreatedOnUtc { get; set; }

    public static User Create(FirstName firstName, LastName lastName, Email email, string passwordHash)
    {
        return new User(firstName, lastName, email, passwordHash);
    }
}
