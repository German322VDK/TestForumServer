using TestForumServer.Domian.Entities.ForumEntities.Contents;

namespace TestForumServer.Domian.Entities.ForumEntities.Likes
{
    /// <summary>
    /// Represents a like for a specific comment.
    /// Inherits from <see cref="LikeEntity"/>.
    /// </summary>
    public class CommentLikeEntity : LikeEntity
    {
        /// <summary>
        /// Gets or sets the ID of the comment that was liked.
        /// </summary>
        public int CommentId { get; set; }

        /// <summary>
        /// Navigation property for the comment that was liked.
        /// </summary>
        public virtual CommentEntity Comment { get; set; }
    }
}
