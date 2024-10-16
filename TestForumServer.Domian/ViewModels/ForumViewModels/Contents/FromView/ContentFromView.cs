using Microsoft.AspNetCore.Http;

namespace TestForumServer.Domian.ViewModels.ForumViewModels.Contents.FromView
{
    /// <summary>
    /// Base class for content form view models.
    /// Contains properties for text content and an optional file upload.
    /// </summary>
    public class ContentFromView
    {
        /// <summary>
        /// Gets or sets the text content.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the file to be uploaded.
        /// </summary>
        public IFormFile? File { get; set; }
    }
}
