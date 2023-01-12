using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Application.Core.Abstractions.Cryptography;

public interface IPasswordHasher
{
    string HashPassword(Password password);
}
