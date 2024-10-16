using TestForumServer.Domian.ViewModels.Identities;

namespace TestForumServer.Domian.ViewModels.ForumViewModels.Contents.Base
{
    /// <summary>
    /// Base class for content view models.
    /// Contains common properties for content entities, including ID, content, creation date, author, and likes.
    /// </summary>
    public abstract class ContentViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the content.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the content text.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the creation date of the content.
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the image associated with the content.
        /// </summary>
        public ImageViewModel? Image { get; set; }

        /// <summary>
        /// Gets or sets the author of the content.
        /// </summary>
        public UserContentViewModel Author { get; set; }

        /// <summary>
        /// Gets or sets the total number of likes for the content.
        /// </summary>
        public int LikesCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is liked by the current user.
        /// </summary>
        public bool IsLiked { get; set; }
    }
}
