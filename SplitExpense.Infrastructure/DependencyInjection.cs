﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SplitExpense.Application.Core.Abstractions.Authentication;
using SplitExpense.Application.Core.Abstractions.Common;
using SplitExpense.Application.Core.Abstractions.Cryptography;
using SplitExpense.Domain.Services;
using SplitExpense.Infrastructure.Authentication;
using SplitExpense.Infrastructure.Authentication.Settings;
using SplitExpense.Infrastructure.Common;
using SplitExpense.Infrastructure.Cryptography;
using System.Text;

namespace SplitExpense.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"]))
            });

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SettingsKey));

        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddTransient<IDateTime, MachineDateTime>();

        services.AddTransient<IPasswordHasher, PasswordHasher>();

        services.AddTransient<IPasswordHashChecker, PasswordHasher>();

        return services;
    }
}