﻿using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Domain.Entities;

public sealed class Expense : AggregateRoot
{
    public Expense(TotalExpense totalExpense, bool paid, Guid userGroupId)
        : base(Guid.NewGuid())
    {
        TotalExpense = totalExpense;
        Paid = paid;
        UserGroupId = userGroupId;
    }

    private Expense() { }

    public TotalExpense TotalExpense { get; private set; }
    public bool Paid { get; private set; }
    public Guid UserGroupId { get; private set; }

    public static Expense Create(TotalExpense totalExpense, bool paid, Guid userGroupId) =>
        new(totalExpense, paid, userGroupId);
}
