namespace TestForumServer.WebInfrastructure.Security
{
    /// <summary>
    /// Interface for token services providing JWT generation and validation methods.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Validates the provided JWT token.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns><c>true</c> if the token is valid; otherwise, <c>false</c>.</returns>
        bool ValidateToken(string token);

        /// <summary>
        /// Generates a JWT token for the specified username.
        /// </summary>
        /// <param name="username">The username for which the token is generated.</param>
        /// <returns>The generated JWT token as a string.</returns>
        string GenerateJwtToken(string username);

    }
}
