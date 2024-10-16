using TestForumServer.Domian.Entities.Identity;
using TestForumServer.Domian.ViewModels.ForumViewModels.Contents.Base;
using TestForumServer.Domian.ViewModels.Identities;

namespace TestForumServer.Infrastructure.Mapping
{
    /// <summary>
    /// Provides mapping methods for converting between <see cref="UserEntity"/> and related view models.
    /// </summary>
    public static class UserMapper
    {
        /// <summary>
        /// Converts a <see cref="UserEntity"/> to a <see cref="UserLongModel"/>.
        /// </summary>
        /// <param name="user">The user entity to convert.</param>
        /// <param name="isAuthor">Indicates if the user is the author.</param>
        /// <param name="webRoot">The root path for accessing images.</param>
        /// <returns>A <see cref="UserLongModel"/> representation of the user entity.</returns>
        public static UserLongModel ToLongModel(this UserEntity user, bool isAuthor, string webRoot ) => new UserLongModel
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            NickName = user.NickName,
            Image = new ImageViewModel(user.ProfilePicturePath, webRoot, $"picture-{user.UserName}"),
            TradsCount = user.Trads.Count,
            PostsCount = user.Posts.Count,
            CommentsCount = user.Comments.Count,
            IsAuthor = isAuthor
        };

        /// <summary>
        /// Converts a <see cref="RegisterViewModel"/> to a <see cref="UserEntity"/>.
        /// </summary>
        /// <param name="model">The register view model to convert.</param>
        /// <param name="defImageName">The default image name to assign.</param>
        /// <returns>A <see cref="UserEntity"/> representation of the register view model.</returns>
        public static UserEntity FromRegToEntity(this RegisterViewModel model, string defImageName) => new UserEntity
        {
            UserName = model.UserName,
            NickName = model.NickName,
            ProfilePicturePath = defImageName
        };

        /// <summary>
        /// Converts a <see cref="UserEntity"/> to a <see cref="UserContentViewModel"/>.
        /// </summary>
        /// <param name="user">The user entity to convert.</param>
        /// <param name="userId">The ID of the user for author verification.</param>
        /// <param name="webRootPath">The root path for accessing images.</param>
        /// <returns>A <see cref="UserContentViewModel"/> representation of the user entity.</returns>
        public static UserContentViewModel ToViewModel(this UserEntity user, int userId, string webRootPath) =>
            new UserContentViewModel
            {
                Id = user.Id,
                Image = new ImageViewModel(user.ProfilePicturePath, webRootPath, $"user-picture-{user.Id}-name"),
                NickName = user.NickName,
                IsAuthor = user.Id == userId,
            };
    }
}
