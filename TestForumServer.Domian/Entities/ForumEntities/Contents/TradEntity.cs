using TestForumServer.Domian.Entities.ForumEntities.Likes;

namespace TestForumServer.Domian.Entities.ForumEntities.Contents
{
    /// <summary>
    /// Represents a trad in the system.
    /// </summary>
    public class TradEntity : ContentEntity
    {
        /// <summary>
        /// Navigation property for the posts associated with the thread.
        /// </summary>
        public virtual ICollection<PostEntity> Posts { get; set; }

        /// <summary>
        /// Navigation property for the likes associated with the thread.
        /// </summary>
        public virtual ICollection<TradLikeEntity> Likes { get; set; }
    }
}
