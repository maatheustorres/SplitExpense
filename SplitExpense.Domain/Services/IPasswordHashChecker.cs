namespace SplitExpense.Domain.Services;

public interface IPasswordHashChecker
{
    bool HashesMatch(string passwordHash, string providedPassword);
}
