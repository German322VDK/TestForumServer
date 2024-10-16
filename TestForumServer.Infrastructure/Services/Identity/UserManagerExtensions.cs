using Microsoft.AspNetCore.Identity;
using TestForumServer.Domian.Entities.Identity;

namespace TestForumServer.Infrastructure.Services.Identity
{
    /// <summary>
    /// Provides extension methods for <see cref="UserManager{TUser}"/> to manage user data.
    /// </summary>
    public static class UserManagerExtensions
    {
        /// <summary>
        /// Updates the profile picture path for a user identified by their username.
        /// </summary>
        /// <param name="userManager">The instance of <see cref="UserManager{TUser}"/>.</param>
        /// <param name="userName">The username of the user whose image is to be updated.</param>
        /// <param name="image">The new image path to set.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="IdentityResult"/> indicating the outcome.</returns>
        public static async Task<IdentityResult> UpdateImage(this UserManager<UserEntity> userManager, string? userName, string image)
        {
            if (userName == null)
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "userName is null",
                    Description = $"Параметр userName=null."
                });

            UserEntity? user = await userManager.FindByNameAsync(userName);
            if (user == null)
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"Пользователь с именем {userName} не найден."
                });

            user.ProfilePicturePath = image;
            return await userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Updates the nickname for a user identified by their username.
        /// </summary>
        /// <param name="userManager">The instance of <see cref="UserManager{TUser}"/>.</param>
        /// <param name="userName">The username of the user whose nickname is to be updated.</param>
        /// <param name="nickName">The new nickname to set.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="IdentityResult"/> indicating the outcome.</returns>
        public static async Task<IdentityResult> UpdateNickName(this UserManager<UserEntity> userManager, string? userName, string nickName)
        {
            if (userName == null)
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "userName is null",
                    Description = $"Параметр userName=null."
                });

            UserEntity? user = await userManager.FindByNameAsync(userName);
            if (user == null)
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"Пользователь с именем {userName} не найден."
                });

            user.NickName = nickName;
            return await userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Checks if a user exists in the system by their username.
        /// </summary>
        /// <param name="userManager">The instance of <see cref="UserManager{TUser}"/>.</param>
        /// <param name="userName">The username to check for existence.</param>
        /// <returns>A task representing the asynchronous operation, returning true if the user exists; otherwise, false.</returns>
        public static async Task<bool> IsUserExist(this UserManager<UserEntity> userManager, string? userName) =>
            !string.IsNullOrWhiteSpace(userName) && await userManager.FindByNameAsync(userName) != null;

    }
}
