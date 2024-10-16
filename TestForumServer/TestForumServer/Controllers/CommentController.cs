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
    /// Controller for managing comments in the system.
    /// </summary>
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IStore<CommentEntity> _commentStore;
        private readonly IStore<PostEntity> _postStore;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IImageManager _imageManager;
        private readonly ILogger<PostController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentController"/> class.
        /// </summary>
        /// <param name="commentStore">Storage for comments.</param>
        /// <param name="postStore">Storage for posts.</param>
        /// <param name="userManager">User manager.</param>
        /// <param name="env">Web hosting environment.</param>
        /// <param name="imageManager">Image manager.</param>
        /// <param name="logger">Logger.</param>
        public CommentController(IStore<CommentEntity> commentStore,
            IStore<PostEntity> postStore,
            UserManager<UserEntity> userManager,
            IWebHostEnvironment env,
            IImageManager imageManager,
            ILogger<PostController> logger)
        {
            _commentStore = commentStore;
            _postStore = postStore;
            _userManager = userManager;
            _env = env;
            _imageManager = imageManager;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new comment to a post.
        /// </summary>
        /// <param name="model">Data for adding a comment.</param>
        /// <returns>
        /// A <see cref="Task{IActionResult}"/> representing the asynchronous operation.
        /// Returns <c>200 OK</c> with the added comment's view model if successful,
        /// <c>404 Not Found</c> if the user or post is not found,
        /// or <c>500 Internal Server Error</c> in case of an error during the operation.
        /// </returns>
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> Add(CommentFromView model)
        {
            UserEntity? user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                return NotFound(new { message = "Пользователь не найден." });
            }

            PostEntity? post = _postStore.Get(model.PostId);
            if (post == null)
            {
                _logger.LogWarning($"Пост id:{model.PostId} не найден.");
                return NotFound(new { message = "Трэд не найден." });
            }

            CommentEntity comment = new CommentEntity
            {
                Content = model.Text,
                UserId = user.Id,
                PostId = model.PostId
            };
            CommentEntity addedComment = await _commentStore.AddAsync(comment);
            CommentEntity updatedComment = addedComment;

            if (model.File != null)
            {
                ImageUploadResult result = await _imageManager
                    .SaveCommentImageAsync(model.File, addedComment.Post.TradId, addedComment.PostId, addedComment.Id);
                if (result.Success)
                {
                    comment.ProfilePicturePath = result.FilePath;
                    updatedComment = await _commentStore.UpdateImageAsync(addedComment.Id, result.FilePath) ?? addedComment;
                }
                else
                {
                    _logger.LogWarning(result.ErrorMessage);
                }
            }
            return Ok(updatedComment.ToViewModel(user.Id, _env.WebRootPath));
        }


        /// <summary>
        /// Deletes a comment by the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the comment.</param>
        /// <returns>
        /// A <see cref="Task{IActionResult}"/> representing the asynchronous operation.
        /// Returns <c>200 OK</c> if the deletion was successful,
        /// <c>404 Not Found</c> if the comment is not found,
        /// <c>403 Forbidden</c> if the user does not have permission to delete the comment,
        /// or <c>500 Internal Server Error</c> in case of an error during the deletion process.
        /// </returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            string userName = User.Identity!.Name!;
            CommentEntity? comment = _commentStore.Get(id);
            if (comment == null)
            {
                _logger.LogWarning($"Коммент с id:{id} не найден.");
                return NotFound(new { message = $"Коммент с id:{id} не найден." });
            }

            int tradId = comment.Post.TradId;
            int postId = comment.PostId;

            if (userName != comment.User.UserName)
            {
                _logger.LogWarning($"Удаление коммента:{id} пользователем {userName} невозможно, у него нет прав");
                return Forbid();
            }

            _logger.LogInformation($"Удаление коммента:{id} пользователем {userName}");
            bool success = await _commentStore.DeleteAsync(id);

            if (!success)
            {
                _logger.LogError($"Ошибка при удалении коммента {id}.");
                return StatusCode(500, "Ошибка при удалении коммента.");
            }
            bool deletedImageResult = await _imageManager.DeleteCommentImageAsync(tradId, postId, id);

            return Ok(true); // HTTP 200
        }

        /// <summary>
        /// Likes a comment.
        /// </summary>
        /// <param name="id">The identifier of the comment.</param>
        /// <returns>
        /// A <see cref="Task{IActionResult}"/> representing the asynchronous operation.
        /// Returns <c>200 OK</c> with the result of the like operation (true/false),
        /// or <c>404 Not Found</c> if the user is not found.
        /// </returns>
        [Authorize]
        [HttpGet("likecomment")]
        public async Task<IActionResult> LikeComment([FromQuery] int id)
        {
            string? userName = User.Identity!.Name!;
            UserEntity? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                return NotFound(new { message = "Пользователь не найден." });
            }

            _logger.LogInformation($"лайк коммента:{id} пользователем {userName}");

            bool result = await _commentStore.LikeContentAsync(id, user.Id);
            return Ok(result);
        }

        /// <summary>
        /// Unlikes a comment.
        /// </summary>
        /// <param name="id">The identifier of the comment.</param>
        /// <returns>
        /// A <see cref="Task{IActionResult}"/> representing the asynchronous operation.
        /// Returns <c>200 OK</c> with the result of the unlike operation (true/false),
        /// or <c>404 Not Found</c> if the user is not found.
        /// </returns>
        [Authorize]
        [HttpGet("unlikecomment")]
        public async Task<IActionResult> UnLikeComment([FromQuery] int id)
        {
            string? userName = User.Identity!.Name!;
            UserEntity? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                return NotFound(new { message = "Пользователь не найден." });
            }

            _logger.LogInformation($"Удаление лайка коммента:{id} пользователем {userName}");

            bool result = await _commentStore.UnLikeContentAsync(id, user.Id);
            return Ok(result);
        }
    }
}
