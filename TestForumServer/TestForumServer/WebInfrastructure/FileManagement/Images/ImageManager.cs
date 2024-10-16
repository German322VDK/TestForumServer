namespace TestForumServer.WebInfrastructure.FileManagement.Images
{
    /// <summary>
    /// Manages image uploads and deletions for various entities such as trads, posts, comments, and users.
    /// </summary>
    public class ImageManager : IImageManager
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ImageManager> _logger;

        private const string UPLOADS_DIRECTORY = "uploads";

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageManager"/> class.
        /// </summary>
        /// <param name="env">The web hosting environment used to access the web root path.</param>
        /// <param name="logger">Logger for logging image management activities.</param>
        public ImageManager(IWebHostEnvironment env, ILogger<ImageManager> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Deletes the image associated with the specified trad ID.
        /// </summary>
        /// <param name="tradId">The ID of the trad whose image is to be deleted.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating success.</returns>
        public async Task<bool> DeleteTradImageAsync(int tradId)
        {
            string tradImagePrefix = $"trad-{tradId}";
            string tradsFolder = Path.Combine(_env.WebRootPath, UPLOADS_DIRECTORY, "trads");
            return DeleteExistingFiles(tradsFolder, tradImagePrefix); 
        }


        /// <summary>
        /// Deletes the image associated with the specified trad ID and post ID.
        /// </summary>
        /// <param name="tradId">The ID of the trad whose post image is to be deleted.</param>
        /// <param name="postId">The ID of the post whose image is to be deleted.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating success.</returns>
        public async Task<bool> DeletePostImageAsync(int tradId, int postId)
        {
            string postImagePrefix = $"trad-{tradId}-post-{postId}";
            string tradsFolder = Path.Combine(_env.WebRootPath, UPLOADS_DIRECTORY, "trads");
            return DeleteExistingFiles(tradsFolder, postImagePrefix);
        }

        /// <summary>
        /// Deletes the image associated with the specified trad ID, post ID, and comment ID.
        /// </summary>
        /// <param name="tradId">The ID of the trad whose comment image is to be deleted.</param>
        /// <param name="postId">The ID of the post whose comment image is to be deleted.</param>
        /// <param name="commentId">The ID of the comment whose image is to be deleted.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating success.</returns>
        public async Task<bool> DeleteCommentImageAsync(int tradId, int postId, int commentId)
        {
            string commentImagePrefix = $"trad-{tradId}-post-{postId}-com-{commentId}";
            string tradsFolder = Path.Combine(_env.WebRootPath, UPLOADS_DIRECTORY, "trads");
            return DeleteExistingFiles(tradsFolder, commentImagePrefix);
        }

        /// <summary>
        /// Saves an image associated with a trad.
        /// </summary>
        /// <param name="image">The image file to be saved.</param>
        /// <param name="tradId">The ID of the trad to which the image is associated.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the result of the image upload.</returns>
        public async Task<ImageUploadResult> SaveTradImageAsync(IFormFile? image, int tradId)
        {
            string fileNamePrefix = $"trad-{tradId}";
            return await SaveImageAsync(image, "trads", fileNamePrefix);
        }

        /// <summary>
        /// Saves an image associated with a post.
        /// </summary>
        /// <param name="image">The image file to be saved.</param>
        /// <param name="tradId">The ID of the trad to which the post is associated.</param>
        /// <param name="postId">The ID of the post to which the image is associated.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the result of the image upload.</returns>
        public async Task<ImageUploadResult> SavePostImageAsync(IFormFile? image, int tradId, int postId)
        {
            string fileNamePrefix = $"trad-{tradId}-post-{postId}";
            return await SaveImageAsync(image, "trads", fileNamePrefix);
        }

        /// <summary>
        /// Saves an image associated with a comment.
        /// </summary>
        /// <param name="image">The image file to be saved.</param>
        /// <param name="tradId">The ID of the trad to which the post is associated.</param>
        /// <param name="postId">The ID of the post to which the comment is associated.</param>
        /// <param name="commentId">The ID of the comment to which the image is associated.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the result of the image upload.</returns>
        public async Task<ImageUploadResult> SaveCommentImageAsync(IFormFile? image, int tradId, int postId, int commentId)
        {
            string fileNamePrefix = $"trad-{tradId}-post-{postId}-com-{commentId}";
            return await SaveImageAsync(image, "trads", fileNamePrefix);
        }

        /// <summary>
        /// Saves an image associated with a user.
        /// </summary>
        /// <param name="image">The image file to be saved.</param>
        /// <param name="userName">The username of the user.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the result of the image upload.</returns>
        public async Task<ImageUploadResult> SaveUserImageAsync(IFormFile? image, string userName)
        {
            string existingFiles = Path.Combine(_env.WebRootPath, UPLOADS_DIRECTORY, "users");
            DeleteExistingFiles(existingFiles, userName);

            return await SaveImageAsync(image, "users", userName);
        }

        private async Task<ImageUploadResult> SaveImageAsync(IFormFile? image,
            string uploadsSubfolder,
            string fileNamePrefix = "")
        {
            if (image == null)
                return new ImageUploadResult(true);

            try
            {
                // Получаем расширение файла
                string fileExtension = Path.GetExtension(image.FileName).ToLower();

                // Проверяем допустимые форматы изображений
                string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];
                if (!allowedExtensions.Contains(fileExtension))
                    return new ImageUploadResult(false, errorMessage: "Недопустимый формат файла.");

                string uploadsFolder = Path.Combine(_env.WebRootPath, UPLOADS_DIRECTORY, uploadsSubfolder);

                string uniqueFileName = $"{fileNamePrefix}{fileExtension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                string relativePath = $"{UPLOADS_DIRECTORY}/{uploadsSubfolder}/{uniqueFileName}";

                // Создаем папку, если ее нет
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Сохраняем файл
                await using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                _logger.LogInformation($"Изображение было создано: {relativePath}");
                return new ImageUploadResult(true, filePath: relativePath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Ошибка в создании изображения: {ex.Message}");
                return new ImageUploadResult(false, errorMessage: ex.Message);
            }
        }

        private bool DeleteExistingFiles(string uploadsFolder, string fileNamePrefix)
        {
            // Удаление существующих изображений
            IEnumerable<string> existingFiles = Directory.GetFiles(uploadsFolder)
                .Where(file => Path.GetFileName(file).StartsWith(fileNamePrefix));
            foreach (string existingFile in existingFiles)
            {
                File.Delete(existingFile);
                _logger.LogInformation($"Удалено существующее изображение: {existingFile}");
            }

            return true;
        }
    }
}
