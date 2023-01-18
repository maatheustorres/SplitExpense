namespace SplitExpense.Contracts.Users;

public sealed class UserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string FistName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
