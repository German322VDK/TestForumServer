namespace TestForumServer.WebInfrastructure.Middlewares
{
    /// <summary>
    /// Middleware for handling exceptions in an ASP.NET Core application.
    /// Catches all unhandled exceptions, logs them, and redirects the user to an error page.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        /// <param name="logger">The logger for logging error information.</param>
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Processes an incoming HTTP request.
        /// Catches exceptions that occur during the request processing.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                if (!context.Response.HasStarted)
                {
                    HandleException(error, context);
                    context.Response.Redirect("/Home/Error");
                }
            }
        }

        private void HandleException(Exception error, HttpContext context)
        {
            string errorMessage = $"\u001b[38;5;196m{error.ToString()}\u001b[0m"; // Цвет: яркий красный
            _logger.LogError(errorMessage);
        }
    }
}
