﻿using SplitExpense.Domain.Entities;

namespace SplitExpense.Domain.Repositories;

public interface IExpenseRepository
{
    void Insert(Expense expense);
}
