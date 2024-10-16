using TestForumServer.Domian.Entities.ForumEntities.Likes;

namespace TestForumServer.Domian.Entities.ForumEntities.Contents
{
    /// <summary>
    /// Represents a comment in the system, associated with a post.
    /// </summary>
    public class CommentEntity : ContentEntity
    {
        /// <summary>
        /// Gets or sets the ID of the post to which the comment belongs.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Navigation property to the post that the comment is associated with.
        /// </summary>
        public virtual PostEntity Post { get; set; }

        /// <summary>
        /// Navigation property for the likes associated with the comment.
        /// </summary>
        public virtual ICollection<CommentLikeEntity> Likes { get; set; }
    }
}
