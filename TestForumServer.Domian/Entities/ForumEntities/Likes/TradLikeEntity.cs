using TestForumServer.Domian.Entities.ForumEntities.Contents;

namespace TestForumServer.Domian.Entities.ForumEntities.Likes
{
    /// <summary>
    /// Represents a like for a specific trad.
    /// Inherits from <see cref="LikeEntity"/>.
    /// </summary>
    public class TradLikeEntity : LikeEntity
    {
        /// <summary>
        /// Gets or sets the ID of the trad that was liked.
        /// </summary>
        public int TradId { get; set; }

        /// <summary>
        /// Navigation property for the trad that was liked.
        /// </summary>
        public virtual TradEntity Trad { get; set; }
    }
}
