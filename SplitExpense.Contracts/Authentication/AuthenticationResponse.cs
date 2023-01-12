namespace SplitExpense.Contracts.Authentication;

public sealed class AuthenticationResponse
{
    public AuthenticationResponse(Guid id, string firstName, string lastName, string fullName, string email, string token)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        FullName = fullName;
        Email = email;
        Token = token;
    }

    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}
