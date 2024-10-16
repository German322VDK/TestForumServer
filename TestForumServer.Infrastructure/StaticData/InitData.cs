namespace TestForumServer.Infrastructure.StaticData
{
    /// <summary>
    /// Contains initialization data constants used for setting up default user, thread, post, and comment data in the application.
    /// </summary>
    public class InitData
    {
        /// <summary>
        /// The default username for the main user.
        /// </summary>
        public const string MAIN_USER_NAME = "main";

        /// <summary>
        /// The default password for the main user.
        /// </summary>
        public const string MAIN_USER_PASS = "mainPAss";

        /// <summary>
        /// The path to the main user's profile picture.
        /// </summary>
        public const string MAIN_USER_PICTURE = "uploads/users/main.jpg";

        /// <summary>
        /// The path to the default user picture.
        /// </summary>
        public const string DEFAULT_USER_PICTURE = "uploads/users/default.jpg";


        /// <summary>
        /// The content of the main thread.
        /// </summary>
        public const string MAIN_TRAD_CONTENT = "Главный трэд";

        /// <summary>
        /// The path to the main thread's picture.
        /// </summary>
        public const string MAIN_TRAD_PICTURE = "uploads/trads/trad-1.jpg";


        /// <summary>
        /// The content of the main post.
        /// </summary>
        public const string MAIN_POST_CONTENT = "Главный пост";

        /// <summary>
        /// The path to the main post's picture.
        /// </summary>
        public const string MAIN_POST_PICTURE = "uploads/trads/trad-1-post-1.jpg";


        /// <summary>
        /// The content of the main comment.
        /// </summary>
        public const string MAIN_COMMENT_CONTENT = "Главный коммент";

        /// <summary>
        /// The path to the main comment's picture.
        /// </summary>
        public const string MAIN_COMMENT_PICTURE = "uploads/trads/trad-1-post-1-com-1.jpg";

    }
}
