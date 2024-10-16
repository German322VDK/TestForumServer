using Microsoft.AspNetCore.Identity;

namespace TestForumServer.Domian.Entities.Identity
{
    /// <summary>
    /// Represents a role in the identity system.
    /// Inherits from <see cref="IdentityRole{TKey}"/> where TKey is of type int.
    /// </summary>
    public class RoleEntity : IdentityRole<int>
    {
    }

    /// <summary>
    /// Defines the different statuses a user can have in the system.
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// User with administrative privileges.
        /// </summary>
        Admin = 1,

        /// <summary>
        /// Regular user with standard privileges.
        /// </summary>
        User = 2,

        /// <summary>
        /// User who has been banned from the system.
        /// </summary>
        Banned = 3
    }
}
