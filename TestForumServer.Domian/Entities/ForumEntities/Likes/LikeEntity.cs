using TestForumServer.Domian.Entities.Identity;

namespace TestForumServer.Domian.Entities.ForumEntities.Likes
{
    /// <summary>
    /// Base class representing a like entity.
    /// Contains the user ID and the associated user entity.
    /// </summary>
    public class LikeEntity
    {
        /// <summary>
        /// Gets or sets the ID of the user who liked the content.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Navigation property for the user who liked the content.
        /// </summary>
        public virtual UserEntity User { get; set; }
    }
}
