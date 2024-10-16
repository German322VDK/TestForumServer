using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.Entities.Identity;
using TestForumServer.Domian.ViewModels.ForumViewModels.Contents.FromView;
using TestForumServer.Infrastructure.Mapping;
using TestForumServer.Infrastructure.Services.Stores.Contents;
using TestForumServer.WebInfrastructure.FileManagement.Images;

namespace TestForumServer.Controllers
{
    /// <summary>
    /// Controller for managing posts in the system.
    /// </summary>
    [Route("api/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IStore<PostEntity> _postStore;
        private readonly IStore<TradEntity> _tradStore;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IImageManager _imageManager;
        private readonly ILogger<PostController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostController"/> class.
        /// </summary>
        /// <param name="postStore">Storage for posts.</param>
        /// <param name="tradStore">Storage for trads.</param>
        /// <param name="userManager">User manager.</param>
        /// <param name="env">Web hosting environment.</param>
        /// <param name="imageManager">Image manager.</param>
        /// <param name="logger">Logger.</param>
        public PostController(IStore<PostEntity> postStore,
            IStore<TradEntity> tradStore,
            UserManager<UserEntity> userManager,
            IWebHostEnvironment env,
            IImageManager imageManager,
            ILogger<PostController> logger)
        {
            _postStore = postStore;
            _tradStore = tradStore;
            _userManager = userManager;
            _env = env;
            _imageManager = imageManager;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new post to a trad.
        /// </summary>
        /// <param name="model">Data for adding a post.</param>
        /// <returns>
        /// A <see cref="Task{IActionResult}"/> representing the asynchronous operation.
        /// Returns <c>200 OK</c> with the added post's view model if successful,
        /// <c>404 Not Found</c> if the user or trad is not found,
        /// or <c>500 Internal Server Error</c> in case of an error during the operation.
        /// </returns>
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> Add(PostFromView model)
        {
            UserEntity? user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                return NotFound(new { message = "Пользователь не найден." });
            }

            TradEntity? trad = _tradStore.Get(model.TradId);
            if(trad == null)
            {
                _logger.LogWarning($"Трэд id:{model.TradId} не найден.");
                return NotFound(new { message = "Трэд не найден." });
            }

            PostEntity post = new PostEntity
            {
                Content = model.Text,
                UserId = user.Id,
                TradId = model.TradId
            };

            PostEntity addedPost = await _postStore.AddAsync(post);
            PostEntity updatedPost = addedPost;
            if (model.File != null)
            {
                ImageUploadResult result = await _imageManager.SavePostImageAsync(model.File, addedPost.TradId, addedPost.Id);

                if (result.Success)
                {
                    updatedPost = await _postStore.UpdateImageAsync(addedPost.Id, result.FilePath) ?? addedPost;
                }
                else
                {
                    _logger.LogWarning(result.ErrorMessage);
                }
            }
            return Ok(updatedPost.ToViewModel(user.Id, _env.WebRootPath));
        }


        /// <summary>
        /// Deletes a post by the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the post.</param>
        /// <returns>
        /// A <see cref="Task{IActionResult}"/> representing the asynchronous operation.
        /// Returns <c>200 OK</c> if the deletion was successful,
        /// <c>404 Not Found</c> if the post is not found,
        /// <c>403 Forbidden</c> if the user does not have permission to delete the post,
        /// or <c>500 Internal Server Error</c> in case of an error during the deletion process.
        /// </returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            string userName = User.Identity!.Name!;
            PostEntity? post = _postStore.Get(id);
            if (post == null)
            {
                _logger.LogWarning($"Пост с id:{id} не найден.");
                return NotFound(new { message = $"Пост с id:{id} не найден." });
            }
            int tradId = post.TradId;

            if (userName != post.User.UserName)
            {
                _logger.LogWarning($"Удаление поста:{id} пользователем {userName} невозможно, у него нет прав");
                return Forbid();
            }

            _logger.LogInformation($"Удаление поста:{id} пользователем {userName}");
            bool success = await _postStore.DeleteAsync(id);

            if (!success)
            {
                _logger.LogError($"Ошибка при удалении поста {id}.");
                return StatusCode(500, "Ошибка при удалении поста.");
            }

            bool deletedImageResult = await _imageManager.DeletePostImageAsync(tradId, id);

            return Ok(true); // HTTP 200
        }

        /// <summary>
        /// Likes a post.
        /// </summary>
        /// <param name="id">The identifier of the post.</param>
        /// <returns>
        /// A <see cref="Task{IActionResult}"/> representing the asynchronous operation.
        /// Returns <c>200 OK</c> with the result of the like operation (true/false),
        /// or <c>404 Not Found</c> if the user is not found.
        /// </returns>
        [Authorize]
        [HttpGet("likepost")]
        public async Task<IActionResult> LikePost([FromQuery] int id)
        {
            string? userName = User.Identity!.Name!;
            UserEntity? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                return NotFound(new { message = "Пользователь не найден." });
            }

            _logger.LogInformation($"лайк поста:{id} пользователем {userName}");

            bool result = await _postStore.LikeContentAsync(id, user.Id);
            return Ok(result);
        }

        /// <summary>
        /// Unlikes a post.
        /// </summary>
        /// <param name="id">The identifier of the post.</param>
        /// <returns>
        /// A <see cref="Task{IActionResult}"/> representing the asynchronous operation.
        /// Returns <c>200 OK</c> with the result of the unlike operation (true/false),
        /// or <c>404 Not Found</c> if the user is not found.
        /// </returns>
        [Authorize]
        [HttpGet("unlikepost")]
        public async Task<IActionResult> UnLikePost([FromQuery] int id)
        {
            string? userName = User.Identity!.Name!;
            UserEntity? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                return NotFound(new { message = "Пользователь не найден." });
            }

            _logger.LogInformation($"Удаление лайка поста:{id} пользователем {userName}");

            bool result = await _postStore.UnLikeContentAsync(id, user.Id);
            return Ok(result);
        }
    }
}
