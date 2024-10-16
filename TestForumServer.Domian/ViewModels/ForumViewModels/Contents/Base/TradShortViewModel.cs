namespace TestForumServer.Domian.ViewModels.ForumViewModels.Contents.Base
{
    /// <summary>
    /// Represents a short trad view model.
    /// Inherits from <see cref="ContentViewModel"/>.
    /// Contains additional properties for comment and post counts.
    /// </summary>
    public class TradShortViewModel : ContentViewModel
    {
        /// <summary>
        /// Gets or sets the number of comments associated with the trad.
        /// </summary>
        public int CommentsCount { get; set; }

        /// <summary>
        /// Gets or sets the number of posts associated with the trad.
        /// </summary>
        public int PostsCount { get; set; }
    }
}
