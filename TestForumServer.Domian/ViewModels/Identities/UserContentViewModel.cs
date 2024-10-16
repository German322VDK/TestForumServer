using TestForumServer.Domian.ViewModels.ForumViewModels.Contents.Base;

namespace TestForumServer.Domian.ViewModels.Identities
{
    /// <summary>
    /// Represents the content view model for user data.
    /// Contains properties for user ID, nickname, image, and author status.
    /// </summary>
    public class UserContentViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the nickname of the user.
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets the image associated with the user.
        /// </summary>
        public ImageViewModel Image { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is the author of the content.
        /// </summary>
        public bool IsAuthor { get; set; }
    }
}
