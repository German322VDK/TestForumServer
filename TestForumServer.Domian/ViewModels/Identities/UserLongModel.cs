namespace TestForumServer.Domian.ViewModels.Identities
{
    /// <summary>
    /// Represents the long-form view model for user data.
    /// Inherits from <see cref="UserContentViewModel"/>.
    /// Contains additional properties for username, and counts of trads, posts, and comments.
    /// </summary>
    public class UserLongModel : UserContentViewModel
    {
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the count of trads created by the user.
        /// </summary>
        public int TradsCount { get; set; }

        /// <summary>
        /// Gets or sets the count of posts created by the user.
        /// </summary>
        public int PostsCount { get; set; }

        /// <summary>
        /// Gets or sets the count of comments made by the user.
        /// </summary>
        public int CommentsCount { get; set; }
    }
}
