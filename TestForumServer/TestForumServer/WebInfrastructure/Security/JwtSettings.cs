namespace TestForumServer.WebInfrastructure.Security
{
    /// <summary>
    /// Represents the settings for JWT (JSON Web Token) configuration.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the secret key used for signing the JWT.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the issuer of the JWT.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the audience for whom the JWT is intended.
        /// </summary>
        public string Audience { get; set; }
    }
}
