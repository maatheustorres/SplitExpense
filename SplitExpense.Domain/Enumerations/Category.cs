using SplitExpense.Domain.Core.Primitives;

namespace SplitExpense.Domain.Enumerations;

public sealed class Category : Enumeration<Category>
{
    public static readonly Category None = new(0, "None");
    public static readonly Category Travel = new(1, "Viagem");
    public static readonly Category Home = new(2, "Casa");

    private Category(int value, string name)
        : base(value, name)
    {
    }

    private Category(int value)
        : base(value, FromValue(value).Name)
    {
    }
}
