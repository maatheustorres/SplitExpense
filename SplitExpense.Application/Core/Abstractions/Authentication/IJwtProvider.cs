
using SplitExpense.Domain.Entities;

namespace SplitExpense.Application.Core.Abstractions.Authentication;

public interface IJwtProvider
{
    string Create(User user);
}
