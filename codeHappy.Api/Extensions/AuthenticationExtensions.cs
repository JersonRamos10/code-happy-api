using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace codeHappy.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddSupabaseAuth(this IServiceCollection services, IConfiguration config)
    {
        var supabaseUrl = config["Supabase:AuthUrl"];
        var authority = $"{supabaseUrl}/auth/v1";
        var metadataAddress = $"{authority}/.well-known/openid-configuration";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Usar ConfigurationManager para obtener las claves JWKS de Supabase
            options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                metadataAddress,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever { RequireHttps = true });

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authority,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                RoleClaimType = "role"
            };
        });

        services.AddAuthorization();
        return services;
    }
}