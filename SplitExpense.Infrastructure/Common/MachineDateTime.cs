using SplitExpense.Application.Core.Abstractions.Common;

namespace SplitExpense.Infrastructure.Common;

internal sealed class MachineDateTime : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}
