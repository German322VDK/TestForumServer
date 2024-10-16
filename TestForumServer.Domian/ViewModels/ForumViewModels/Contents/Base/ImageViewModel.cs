namespace TestForumServer.Domian.ViewModels.ForumViewModels.Contents.Base
{
    /// <summary>
    /// Represents an image view model.
    /// Contains properties for image name, data, and file extension.
    /// </summary>
    public class ImageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageViewModel"/> class.
        /// </summary>
        /// <param name="localFilePath">The local path to the image file.</param>
        /// <param name="webRootPath">The web root path for file storage.</param>
        /// <param name="imageName">The name of the image file.</param>
        public ImageViewModel(string localFilePath, string webRootPath, string imageName)
        {
            var filePath = Path.Combine(webRootPath, localFilePath);

            var fileBytes = File.ReadAllBytes(filePath);

            ImageData = Convert.ToBase64String(fileBytes);
            FileExtension = Path.GetExtension(filePath).ToLower();
            ImageName = imageName;
        }

        /// <summary>
        /// Gets the name of the image file.
        /// </summary>
        public string ImageName { get; init; }

        /// <summary>
        /// Gets the base64-encoded image data.
        /// </summary>
        public string ImageData { get; init; }

        /// <summary>
        /// Gets the file extension of the image.
        /// </summary>
        public string FileExtension { get; init; }
    }
}
