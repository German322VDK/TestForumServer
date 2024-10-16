
namespace TestForumServer
{
    /// <summary>
    /// The main class of the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args) =>
            new Startup(args)
            .InitializeApp()
            .Run();
    }
}
