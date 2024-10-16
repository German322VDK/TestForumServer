namespace TestForumServer.WebInfrastructure.FileManagement.Images
{
    /// <summary>
    /// Defines methods for managing image uploads and deletions.
    /// </summary>
    public interface IImageManager
    {
        /// <summary>
        /// Saves an image associated with a trad.
        /// </summary>
        /// <param name="image">The image file to be saved.</param>
        /// <param name="tradId">The ID of the trad to which the image is associated.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the result of the image upload.</returns>
        Task<ImageUploadResult> SaveTradImageAsync(IFormFile? image, int tradId);

        /// <summary>
        /// Saves an image associated with a post.
        /// </summary>
        /// <param name="image">The image file to be saved.</param>
        /// <param name="tradId">The ID of the trad to which the post is associated.</param>
        /// <param name="postId">The ID of the post to which the image is associated.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the result of the image upload.</returns>
        Task<ImageUploadResult> SavePostImageAsync(IFormFile? image, int tradId, int postId);

        /// <summary>
        /// Saves an image associated with a comment.
        /// </summary>
        /// <param name="image">The image file to be saved.</param>
        /// <param name="tradId">The ID of the trad to which the post is associated.</param>
        /// <param name="postId">The ID of the post to which the comment is associated.</param>
        /// <param name="commentId">The ID of the comment to which the image is associated.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the result of the image upload.</returns>
        Task<ImageUploadResult> SaveCommentImageAsync(IFormFile? image, int tradId, int postId, int commentId);

        /// <summary>
        /// Saves an image associated with a user.
        /// </summary>
        /// <param name="image">The image file to be saved.</param>
        /// <param name="username">The username of the user.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the result of the image upload.</returns>
        Task<ImageUploadResult> SaveUserImageAsync(IFormFile? image, string username);

        /// <summary>
        /// Deletes the image associated with the specified trad ID.
        /// </summary>
        /// <param name="tradId">The ID of the trad whose image is to be deleted.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating success.</returns>
        Task<bool> DeleteTradImageAsync(int tradId);

        /// <summary>
        /// Deletes the image associated with the specified trad ID and post ID.
        /// </summary>
        /// <param name="tradId">The ID of the trad whose post image is to be deleted.</param>
        /// <param name="postId">The ID of the post whose image is to be deleted.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating success.</returns>
        Task<bool> DeletePostImageAsync(int tradId, int postId);

        /// <summary>
        /// Deletes the image associated with the specified trad ID, post ID, and comment ID.
        /// </summary>
        /// <param name="tradId">The ID of the trad whose comment image is to be deleted.</param>
        /// <param name="postId">The ID of the post whose comment image is to be deleted.</param>
        /// <param name="commentId">The ID of the comment whose image is to be deleted.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating success.</returns>
        Task<bool> DeleteCommentImageAsync(int tradId, int postId, int commentId);
    }
}
