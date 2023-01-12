using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SplitExpense.Application.Core.Abstractions.Authentication;
using SplitExpense.Application.Core.Abstractions.Common;
using SplitExpense.Domain.Entities;
using SplitExpense.Infrastructure.Authentication.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SplitExpense.Infrastructure.Authentication;

internal sealed class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _jwtSettings;
    private readonly IDateTime _dateTime;

    public JwtProvider(IOptions<JwtSettings> jwtSettings, IDateTime dateTime)
    {
        _jwtSettings = jwtSettings.Value;
        _dateTime = dateTime;
    }

    public string Create(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("email", user.Email),
            new Claim("name", user.FullName)
        };

        DateTime tokenExpirationTime = _dateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            null,
            tokenExpirationTime,
            signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}
