using System.Buffers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using ReadingIsGood.Application.Settings;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Helpers;

public static class JwtTokenHelper
{
    private static readonly SymmetricSecurityKey AuthSigningKey = new(Encoding.UTF8.GetBytes(AuthenticationSettings.Secret));
    private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();

    public static string GenerateJwtToken<T>(T jwtTokenModel)
    {
        using var memoryOwner = MemoryPool<Claim>.Shared.Rent(16);
        var authClaims = memoryOwner.Memory.Span;

        var properties = typeof(T).GetProperties();

        int index = 0;
        foreach (var property in properties)
        {
            var value = property.GetValue(jwtTokenModel);
            if (value == null)
                continue;

            if (property.PropertyType == typeof(IList<string>))
                foreach (var role in (IList<string>)value)
                    authClaims[index++] = new Claim(property.Name, role);
            else
                authClaims[index++] = new Claim(property.Name, value.ToString() ?? string.Empty);
        }

        foreach (var validAudience in AuthenticationSettings.ValidAudiences)
            authClaims[index++] = new Claim(JwtRegisteredClaimNames.Aud, validAudience);

        var securityToken = new JwtSecurityToken(
            issuer: AuthenticationSettings.ValidIssuer,
            expires: DateTime.UtcNow.AddHours(AuthenticationSettings.TokenExpirationHour),
            notBefore: DateTime.UtcNow,
            claims: authClaims[..index].ToArray(),
            signingCredentials: new SigningCredentials(AuthSigningKey, SecurityAlgorithms.HmacSha256));

        return JwtSecurityTokenHandler.WriteToken(securityToken);
    }

    public static async ValueTask ContextValidateTokenExtension(this HttpContext context, string? token)
    {
        try
        {
            var validatedToken = await JwtSecurityTokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidAudiences = AuthenticationSettings.ValidAudiences,
                IssuerSigningKey = AuthSigningKey,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = AuthenticationSettings.ValidIssuer,
                ValidateAudience = true,
                ValidAudience = AuthenticationSettings.ValidAudiences.First(),
                ClockSkew = TimeSpan.Zero
            });

            if (validatedToken.IsValid)
                foreach (var claim in validatedToken.Claims)
                    context.Items[claim.Key] = claim.Value;
            else
                context.Request.Headers.Authorization = "";
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(BaseResponse<string>.Fail("Authorization token is either incorrect or missing.", StatusCodes.Status401Unauthorized));
        }
    }
}

public readonly struct DefaultJwtTokenModel()
{
    public DefaultJwtTokenModel(Guid userId, IList<string> roles) : this()
    {
        UserId = userId;
        Roles = roles;
    }

    public Guid UserId { get; init; }
    public IList<string> Roles { get; init; } = [];
}