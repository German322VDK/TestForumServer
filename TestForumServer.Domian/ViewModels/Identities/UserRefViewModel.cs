namespace TestForumServer.Domian.ViewModels.Identities
{
    /// <summary>
    /// Represents a reference view model for a user, containing basic user information.
    /// </summary>
    public class UserRefViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRefViewModel"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="userName">The username of the user.</param>
        public UserRefViewModel(int id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string UserName { get; init; }
    }
}
