namespace TestForumServer.Domian.ViewModels.Identities
{
    /// <summary>
    /// Represents the view model for user information, including user credentials and identifiers.
    /// </summary>
    public class UserInfoViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoViewModel"/> class.
        /// </summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <param name="token">The JWT token for the authenticated user.</param>
        /// <param name="userName">The username of the user.</param>
        /// <param name="nickName">The nickname of the user.</param>
        public UserInfoViewModel(int id, string token, string userName, string nickName)
        {
            Id = id;
            Token = token;
            UserName = userName;
            NickName = nickName;
        }

        /// <summary>
        /// Gets or sets the user's unique identifier.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets or sets the JWT token for the authenticated user.
        /// </summary>
        public string Token { get; init; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string UserName { get; init; }

        /// <summary>
        /// Gets or sets the nickname of the user.
        /// </summary>
        public string NickName { get; init; }
    }
}
