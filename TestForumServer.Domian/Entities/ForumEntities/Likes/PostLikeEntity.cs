using TestForumServer.Domian.Entities.ForumEntities.Contents;

namespace TestForumServer.Domian.Entities.ForumEntities.Likes
{
    /// <summary>
    /// Represents a like for a specific post.
    /// Inherits from <see cref="LikeEntity"/>.
    /// </summary>
    public class PostLikeEntity : LikeEntity
    {
        /// <summary>
        /// Gets or sets the ID of the post that was liked.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Navigation property for the post that was liked.
        /// </summary>
        public virtual PostEntity Post { get; set; }
    }
}
