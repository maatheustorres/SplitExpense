﻿using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Services;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Domain.Entities;
public sealed class User : AggregateRoot
{
    private string _passwordHash;

    private User(FirstName firstName, LastName lastName, Email email, string passwordHash)
        : base(Guid.NewGuid())
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        _passwordHash = passwordHash;
    }

    private User() { }

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }

    public string FullName => $"{FirstName} {LastName}";
    public Email Email { get; private set; }
    public IList<UserGroup> UserGroup { get; set; }

    public static User Create(FirstName firstName, LastName lastName, Email email, string passwordHash)
    {
        return new User(firstName, lastName, email, passwordHash);
    }

    public bool VerifyPasswordHash(string password, IPasswordHashChecker passwordHashChecker)
        => password is not null && passwordHashChecker.HashesMatch(_passwordHash, password);
}
