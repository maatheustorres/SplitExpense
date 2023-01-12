namespace SplitExpense.Infrastructure.Authentication.Settings;

public class JwtSettings
{
    public const string SettingsKey = "Jwt";
    public JwtSettings()
    {
        Issuer = string.Empty;
        Audience = string.Empty;
        SecurityKey = string.Empty;
    }

    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecurityKey { get; set; }
    public int TokenExpirationInMinutes { get; set; }
}
