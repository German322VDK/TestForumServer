using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.Entities.Identity;
using TestForumServer.Domian.ViewModels.ForumViewModels.Contents.Base;
using TestForumServer.Domian.ViewModels.ForumViewModels.Contents.FromView;
using TestForumServer.Infrastructure.Mapping;
using TestForumServer.Infrastructure.Services.Stores.Contents;
using TestForumServer.WebInfrastructure.FileManagement.Images;

namespace TestForumServer.Controllers
{
    /// <summary>
    /// Controller for managing trads.
    /// </summary>
    [Route("api/trad")]
    [ApiController]
    public class TradController : ControllerBase
    {
        private readonly IStore<TradEntity> _tradStore;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IImageManager _imageManager;
        private readonly ILogger<TradController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradController"/> class.
        /// </summary>
        /// <param name="tradStore">An instance of <see cref="IStore{TradEntity}"/> for managing trads.</param>
        /// <param name="userManager">An instance of <see cref="UserManager{UserEntity}"/> for user management.</param>
        /// <param name="env">An instance of <see cref="IWebHostEnvironment"/> providing web hosting environment information.</param>
        /// <param name="imageManager">An instance of <see cref="IImageManager"/> for managing images associated with trads.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TradController}"/> for logging operations.</param>
        public TradController(IStore<TradEntity> tradStore,
            UserManager<UserEntity> userManager,
            IWebHostEnvironment env,
            IImageManager imageManager,
            ILogger<TradController> logger)
        {
            _tradStore = tradStore;
            _userManager = userManager;
            _env = env;
            _imageManager = imageManager;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new trad.
        /// </summary>
        /// <param name="model">The model containing the data for the new trad.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation, including the created trad in the response body on success.</returns>
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> Add(TradFromView model)
        {
            UserEntity? user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                return NotFound(new { message = "Пользователь не найден." });
            }

            TradEntity trad = new TradEntity
            {
                Content = model.Text,
                UserId = user.Id,
            };

            TradEntity addedTrad = await _tradStore.AddAsync(trad);
            TradEntity updatedTrad = addedTrad;
            if (model.File != null)
            {
                ImageUploadResult result = await _imageManager.SaveTradImageAsync(model.File, addedTrad.Id);

                if (result.Success)
                {
                    updatedTrad = await _tradStore.UpdateImageAsync(addedTrad.Id, result.FilePath) ?? addedTrad;
                }
                else
                {
                    _logger.LogWarning(result.ErrorMessage);
                }
            }

            return Ok(updatedTrad.ToShortViewModel(user.Id, _env.WebRootPath));
        }

        /// <summary>
        /// Deletes a specific trad by its ID.
        /// </summary>
        /// <param name="id">The ID of the trad to delete.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the success or failure of the deletion operation.</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrad(int id)
        {
            string userName = User.Identity!.Name!;
            TradEntity? trad = _tradStore.Get(id);
            if (trad == null)
            {
                _logger.LogWarning($"Трэд с id:{id} не найден.");
                return NotFound();
            }

            if (userName != trad.User.UserName)
            {
                _logger.LogWarning($"Удаление трэда:{id} пользователем {userName} невозможно, у него нет прав");
                return Forbid();
            }

            _logger.LogInformation($"Удаление трэда:{id} пользователем {userName}");
            bool success = await _tradStore.DeleteAsync(id);

            if (!success)
            {
                _logger.LogError($"Ошибка при удалении трэда {id}.");
                return StatusCode(500, "Ошибка при удалении трэда.");
            }
            bool deletedImageResult = await _imageManager.DeleteTradImageAsync(id);
            return Ok(true); // HTTP 200
        }

        /// <summary>
        /// Retrieves all trads.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing a list of all trads, or a 404 if no trads are found.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            int userId = -1;
            if (User.Identity!.IsAuthenticated)
            {
                UserEntity? user = await _userManager.FindByNameAsync(User.Identity.Name!);

                if(user == null)
                {
                    _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                    return NotFound(new { message = "Пользователь не найден." });
                }
                _logger.LogInformation($"Пользователь:{user.UserName}");
                userId = user.Id;
            }
            IEnumerable<TradViewModel> trads = _tradStore.GetAll().ToViewModel(userId, _env.WebRootPath);
            return Ok(trads);
        }

        /// <summary>
        /// Retrieves all trads in a short format.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing a list of all trads in a short format.</returns>
        [HttpGet("allshort")]
        public async Task<IActionResult> GetAllShort()
        {
            int userId = -1;
            if (User.Identity!.IsAuthenticated)
            {
                UserEntity? user = await _userManager.FindByNameAsync(User.Identity.Name!);

                if (user == null)
                {
                    _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                    return NotFound(new { message = "Пользователь не найден." });
                }
                _logger.LogInformation($"Пользователь:{user.UserName}");
                userId = user.Id;
            }
            IEnumerable<TradShortViewModel> trads = _tradStore.GetAll().ToShortViewModel(userId, _env.WebRootPath);
            return Ok(trads);
        }

        /// <summary>
        /// Retrieves a reference list of all trads, including their IDs and titles.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing a reference list of trads.</returns>
        [HttpGet("allref")]
        public IActionResult GetAllRef() =>
            Ok(_tradStore.GetAll().Select(el => new
            {
                el.Id,
                Title = el.Content,
            }));

        /// <summary>
        /// Retrieves a specific trad by its ID.
        /// </summary>
        /// <param name="id">The ID of the trad to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the requested trad, or a 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            int userId = -1;
            if (User.Identity!.IsAuthenticated)
            {
                UserEntity? user = await _userManager.FindByNameAsync(User.Identity.Name!);

                if (user == null)
                {
                    _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                    return NotFound(new { message = "Пользователь не найден." });
                }
                _logger.LogInformation($"Пользователь:{user.UserName}");
                userId = user.Id;
            }

            TradEntity? tradEntity = _tradStore.Get(id);
            if(tradEntity == null)
                return NotFound(new { message = "Трэд не найден." });

            TradViewModel trad = tradEntity.ToViewModel(userId, _env.WebRootPath);
            return Ok(trad);
        }


        /// <summary>
        /// Likes a specific trad.
        /// </summary>
        /// <param name="id">The ID of the trad to like.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the success or failure of the like operation.</returns>
        [Authorize]
        [HttpGet("liketrad")]
        public async Task<IActionResult> LikeTrad([FromQuery] int id)
        {
            string? userName = User.Identity!.Name!;
            UserEntity? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                return NotFound(new { message = "Пользователь не найден." });
            }

            _logger.LogInformation($"лайк трэда:{id} пользователем {userName}");

            bool result = await _tradStore.LikeContentAsync(id, user.Id);
            return Ok(result);
        }

        /// <summary>
        /// Unlikes a specific trad.
        /// </summary>
        /// <param name="id">The ID of the trad to unlike.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the success or failure of the unlike operation.</returns>
        [Authorize]
        [HttpGet("unliketrad")]
        public async Task<IActionResult> UnLikeTrad([FromQuery] int id)
        {
            string? userName = User.Identity!.Name!;
            UserEntity? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь:{User.Identity.Name} не найден.");
                return NotFound(new { message = "Пользователь не найден." });
            }

            _logger.LogInformation($"Удаление лайка трэда:{id} пользователем {userName}");

            bool result = await _tradStore.UnLikeContentAsync(id, user.Id);
            return Ok(result);
        }
    }
}
