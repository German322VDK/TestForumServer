namespace TestForumServer.WebInfrastructure.FileManagement.Images
{
    /// <summary>
    /// Represents the result of an image upload operation.
    /// </summary>
    public class ImageUploadResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageUploadResult"/> class.
        /// </summary>
        /// <param name="success">Indicates whether the upload was successful.</param>
        /// <param name="filePath">The file path of the uploaded image.</param>
        /// <param name="errorMessage">An optional error message if the upload failed.</param>
        public ImageUploadResult(bool success, string filePath = "", string errorMessage = "")
        {
            Success = success;
            FilePath = filePath;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets a value indicating whether the upload was successful.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Gets the file path of the uploaded image.
        /// </summary>
        public string FilePath { get; init; }

        /// <summary>
        /// Gets an optional error message if the upload failed.
        /// </summary>
        public string ErrorMessage { get; init; }
    }
}
