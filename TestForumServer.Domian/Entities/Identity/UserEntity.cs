using Microsoft.AspNetCore.Identity;
using TestForumServer.Domian.Entities.ForumEntities.Contents;

namespace TestForumServer.Domian.Entities.Identity
{
    /// <summary>
    /// Represents a user in the identity system.
    /// Inherits from <see cref="IdentityUser{TKey}"/> where TKey is of type int.
    /// </summary>
    public class UserEntity : IdentityUser<int>
    {
        /// <summary>
        /// Gets or sets the user's nickname.
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets the path to the user's profile picture.
        /// </summary>
        public string ProfilePicturePath { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for the comments made by the user.
        /// </summary>
        public virtual ICollection<CommentEntity> Comments { get; set; }

        /// <summary>
        /// Navigation property for the trads created by the user.
        /// </summary>
        public virtual ICollection<TradEntity> Trads { get; set; }

        /// <summary>
        /// Navigation property for the posts created by the user.
        /// </summary>
        public virtual ICollection<PostEntity> Posts { get; set; }
    }
}
