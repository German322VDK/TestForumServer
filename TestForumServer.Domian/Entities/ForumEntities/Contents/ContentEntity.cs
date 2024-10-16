using System.ComponentModel.DataAnnotations;
using TestForumServer.Domian.Entities.ForumEntities.Base;
using TestForumServer.Domian.Entities.Identity;

namespace TestForumServer.Domian.Entities.ForumEntities.Contents
{
    /// <summary>
    /// Abstract base class for content-based entities.
    /// Provides a content field and common properties like UserId, ProfilePicturePath, and CreatedAt.
    /// </summary>
    public abstract class ContentEntity : Entity
    {
        /// <summary>
        /// Gets or sets the main content for the entity.
        /// </summary>
        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path to the profile picture associated with the content.
        /// This field is optional.
        /// </summary>
        public string? ProfilePicturePath { get; set; } // Путь к файлу

        /// <summary>
        /// Gets or sets the creation date and time of the entity.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the user ID who created the content.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Navigation property to the user who created the content.
        /// </summary>
        public virtual UserEntity User { get; set; }
    }
}
