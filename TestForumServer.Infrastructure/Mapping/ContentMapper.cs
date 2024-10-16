using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.ViewModels.ForumViewModels.Contents.Base;

namespace TestForumServer.Infrastructure.Mapping
{
    /// <summary>
    /// Provides mapping methods for converting between content entities and their corresponding view models.
    /// </summary>
    public static class ContentMapper
    {
        private const string DEFAULT_DATE_FORMAT = "yyyy.MM.dd:HH.mm.ss";

        #region Trad

        /// <summary>
        /// Converts a <see cref="TradEntity"/> to a <see cref="TradViewModel"/>.
        /// </summary>
        /// <param name="trad">The trad entity to convert.</param>
        /// <param name="userId">The ID of the user for author verification.</param>
        /// <param name="webRootPath">The root path for accessing images.</param>
        /// <returns>A <see cref="TradViewModel"/> representation of the trad entity.</returns>
        public static TradViewModel ToViewModel(this TradEntity trad, int userId, string webRootPath) =>
            new TradViewModel
            {
                Id = trad.Id,
                Content = trad.Content,
                CreatedAt = trad.CreatedAt.ToString(DEFAULT_DATE_FORMAT),
                Image = GetImageViewModel(trad.ProfilePicturePath, webRootPath, $"trad-picture-{trad.Id}-name"),
                Posts = trad.Posts?.ToViewModel(userId, webRootPath) ?? new List<PostViewModel>(),
                Author = trad.User.ToViewModel(userId, webRootPath),
                LikesCount = trad.Likes?.Count ?? 0,
                IsLiked = trad.Likes?.FirstOrDefault(el => el.UserId == userId) != null,
            };

        /// <summary>
        /// Converts a <see cref="TradEntity"/> to a <see cref="TradShortViewModel"/>.
        /// </summary>
        /// <param name="trad">The trad entity to convert.</param>
        /// <param name="userId">The ID of the user for author verification.</param>
        /// <param name="webRootPath">The root path for accessing images.</param>
        /// <returns>A <see cref="TradShortViewModel"/> representation of the trad entity.</returns>
        public static TradShortViewModel ToShortViewModel(this TradEntity trad, int userId, string webRootPath) =>
            new TradShortViewModel
            {
                Id = trad.Id,
                Content = trad.Content,
                CreatedAt = trad.CreatedAt.ToString(DEFAULT_DATE_FORMAT),
                Image = GetImageViewModel(trad.ProfilePicturePath, webRootPath, $"trad-picture-{trad.Id}-name"),
                Author = trad.User.ToViewModel(userId, webRootPath),
                LikesCount = trad.Likes?.Count ?? 0,
                PostsCount = trad.Posts?.Count ?? 0,
                CommentsCount = trad.Posts?.Sum(post => post.Comments?.Count ?? 0) ?? 0,
                IsLiked = trad.Likes?.FirstOrDefault(el => el.UserId == userId) != null,
            };

        /// <summary>
        /// Converts a collection of <see cref="TradEntity"/> to a collection of <see cref="TradViewModel"/>.
        /// </summary>
        /// <param name="trads">The collection of trad entities to convert.</param>
        /// <param name="userId">The ID of the user for author verification.</param>
        /// <param name="webRootPath">The root path for accessing images.</param>
        /// <returns>A collection of <see cref="TradViewModel"/> representations.</returns>
        public static IEnumerable<TradViewModel> ToViewModel(this IEnumerable<TradEntity> trads, int userId, string webRootPath) =>
            trads.Select(tr => tr.ToViewModel(userId, webRootPath));

        /// <summary>
        /// Converts a collection of <see cref="TradEntity"/> to a collection of <see cref="TradShortViewModel"/>.
        /// </summary>
        /// <param name="trads">The collection of trad entities to convert.</param>
        /// <param name="userId">The ID of the user for author verification.</param>
        /// <param name="webRootPath">The root path for accessing images.</param>
        /// <returns>A collection of <see cref="TradShortViewModel"/> representations.</returns>
        public static IEnumerable<TradShortViewModel> ToShortViewModel(this IEnumerable<TradEntity> trads, int userId, string webRootPath) =>
            trads.Select(tr => tr.ToShortViewModel(userId, webRootPath));

        #endregion

        #region Post

        /// <summary>
        /// Converts a <see cref="PostEntity"/> to a <see cref="PostViewModel"/>.
        /// </summary>
        /// <param name="post">The post entity to convert.</param>
        /// <param name="userId">The ID of the user for author verification.</param>
        /// <param name="webRootPath">The root path for accessing images.</param>
        /// <returns>A <see cref="PostViewModel"/> representation of the post entity.</returns>
        public static PostViewModel ToViewModel(this PostEntity post, int userId, string webRootPath) =>
            new PostViewModel
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt.ToString(DEFAULT_DATE_FORMAT),
                Image = GetImageViewModel(post.ProfilePicturePath, webRootPath, $"post-picture-{post.Id}-name"),
                Comments = post.Comments?.ToViewModel(userId, webRootPath) ?? new List<CommentViewModel>(),
                Author = post.User.ToViewModel(userId, webRootPath),
                LikesCount = post.Likes?.Count ?? 0,
                IsLiked = post.Likes?.FirstOrDefault(el => el.UserId == userId) != null,
            };

        /// <summary>
        /// Converts a collection of <see cref="PostEntity"/> to a collection of <see cref="PostViewModel"/>.
        /// </summary>
        /// <param name="posts">The collection of post entities to convert.</param>
        /// <param name="userId">The ID of the user for author verification.</param>
        /// <param name="webRootPath">The root path for accessing images.</param>
        /// <returns>A collection of <see cref="PostViewModel"/> representations.</returns>
        public static IEnumerable<PostViewModel> ToViewModel(this IEnumerable<PostEntity> posts, int userId, string webRootPath) =>
           posts.Select(p => p.ToViewModel(userId, webRootPath));

        #endregion

        #region Comment

        /// <summary>
        /// Converts a <see cref="CommentEntity"/> to a <see cref="CommentViewModel"/>.
        /// </summary>
        /// <param name="comment">The comment entity to convert.</param>
        /// <param name="userId">The ID of the user for author verification.</param>
        /// <param name="webRootPath">The root path for accessing images.</param>
        /// <returns>A <see cref="CommentViewModel"/> representation of the comment entity.</returns>
        public static CommentViewModel ToViewModel(this CommentEntity comment, int userId, string webRootPath) =>
            new CommentViewModel
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt.ToString(DEFAULT_DATE_FORMAT),
                Image = GetImageViewModel(comment.ProfilePicturePath, webRootPath, $"comment-picture-{comment.Id}-name"),
                Author = comment.User.ToViewModel(userId, webRootPath),
                LikesCount = comment.Likes?.Count ?? 0,
                IsLiked = comment.Likes?.FirstOrDefault(el => el.UserId == userId) != null,
            };

        /// <summary>
        /// Converts a collection of <see cref="CommentEntity"/> to a collection of <see cref="CommentViewModel"/>.
        /// </summary>
        /// <param name="comments">The collection of comment entities to convert.</param>
        /// <param name="userId">The ID of the user for author verification.</param>
        /// <param name="webRootPath">The root path for accessing images.</param>
        /// <returns>A collection of <see cref="CommentViewModel"/> representations.</returns>
        public static IEnumerable<CommentViewModel> ToViewModel(this IEnumerable<CommentEntity> comments, int userId, string webRootPath) =>
           comments.Select(cm => cm.ToViewModel(userId, webRootPath));

        #endregion

        /// <summary>
        /// Retrieves an <see cref="ImageViewModel"/> based on the provided local path and web root.
        /// </summary>
        /// <param name="localPath">The local path of the image.</param>
        /// <param name="webRoot">The web root path for accessing images.</param>
        /// <param name="imageName">The name to assign to the image.</param>
        /// <returns>An <see cref="ImageViewModel"/> if the local path is valid; otherwise, null.</returns>
        private static ImageViewModel? GetImageViewModel(string? localPath, string webRoot, string imageName) => !string.IsNullOrEmpty(localPath) ?
            new ImageViewModel(localPath, webRoot, imageName) : null;

    }
}
