namespace TestForumServer.Domian.ViewModels.ForumViewModels.Contents.FromView
{
    /// <summary>
    /// Represents a post form view model.
    /// Inherits from <see cref="ContentFromView"/>.
    /// Contains properties specific to posts, including the associated trad ID.
    /// </summary>
    public class PostFromView : ContentFromView
    {
        /// <summary>
        /// Gets or sets the ID of the trad associated with the post.
        /// </summary>
        public int TradId { get; set; }
    }
}
