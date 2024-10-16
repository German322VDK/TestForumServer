using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace TestForumServer.WebInfrastructure.Security
{
    /// <summary>
    /// Provides custom options for JWT token validation.
    /// </summary>
    public static class CustomTokenOptions
    {
        /// <summary>
        /// Gets the token validation parameters based on the provided issuer, audience, and key.
        /// </summary>
        /// <param name="issuer">The issuer of the token.</param>
        /// <param name="audience">The audience of the token.</param>
        /// <param name="key">The secret key used for signing the token.</param>
        /// <returns>Token validation parameters for validating JWT tokens.</returns>
        public static TokenValidationParameters GetTokenValidationParameters(string issuer, string audience, string key) =>
            GetTokenValidationParameters(issuer, audience, Encoding.UTF8.GetBytes(key));

        /// <summary>
        /// Gets the token validation parameters based on the provided issuer, audience, and byte key.
        /// </summary>
        /// <param name="issuer">The issuer of the token.</param>
        /// <param name="audience">The audience of the token.</param>
        /// <param name="key">The secret key used for signing the token as a byte array.</param>
        /// <returns>Token validation parameters for validating JWT tokens.</returns>
        public static TokenValidationParameters GetTokenValidationParameters(string issuer, string audience, byte[] key) =>
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(0),
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                NameClaimType = ClaimTypes.Name,
            };
    }
}
