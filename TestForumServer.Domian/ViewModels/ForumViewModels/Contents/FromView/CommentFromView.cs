namespace TestForumServer.Domian.ViewModels.ForumViewModels.Contents.FromView
{
    /// <summary>
    /// Represents a comment form view model.
    /// Inherits from <see cref="ContentFromView"/>.
    /// Contains properties specific to comments, including the associated post ID.
    /// </summary>
    public class CommentFromView : ContentFromView
    {
        /// <summary>
        /// Gets or sets the ID of the post associated with the comment.
        /// </summary>
        public int PostId { get; set; }
    }
}
