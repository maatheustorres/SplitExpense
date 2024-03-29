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
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };
        });

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SettingsKey));

        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddTransient<IDateTime, MachineDateTime>();

        services.AddTransient<IPasswordHasher, PasswordHasher>();

        services.AddTransient<IPasswordHashChecker, PasswordHasher>();

        return services;
    }
}
