using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestForumServer.Domian.Entities.Identity;
using TestForumServer.Domian.ViewModels.ForumViewModels.Contents.Base;
using TestForumServer.Domian.ViewModels.Identities;
using TestForumServer.Infrastructure.Mapping;
using TestForumServer.Infrastructure.Services.Identity;
using TestForumServer.Infrastructure.StaticData;
using TestForumServer.WebInfrastructure.FileManagement.Images;
using TestForumServer.WebInfrastructure.Security;

namespace TestForumServer.Controllers
{
    /// <summary>
    /// Controller for managing user-related actions.
    /// </summary>
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly ITokenService _tockenService;
        private readonly IImageManager _imageManager;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="env">The web host environment.</param>
        /// <param name="configuration">The configuration settings.</param>
        /// <param name="userManager">The user manager service.</param>
        /// <param name="signInManager">The sign-in manager service.</param>
        /// <param name="tockenService">The token service for JWT generation.</param>
        /// <param name="imageManager">The image manager service for handling user images.</param>
        /// <param name="logger">The logger service for logging activities.</param>
        public UsersController(IWebHostEnvironment env, 
            IConfiguration configuration,
            UserManager<UserEntity> userManager,
            SignInManager<UserEntity> signInManager,
            ITokenService tockenService,
            IImageManager imageManager,
            ILogger<UsersController> logger)
        {
            _env = env;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _tockenService = tockenService;
            _imageManager = imageManager;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user with the specified login credentials.
        /// </summary>
        /// <param name="model">The login view model containing the username and password.</param>
        /// <returns>A JWT token and user information if successful, otherwise an error message.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            UserEntity? user = await _userManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                _logger.LogWarning($"Пользователь:{model.UserName} не найден.");
                return NotFound(new { message = "Пользователь не найден." });
            }

            var login_result = await _signInManager.PasswordSignInAsync(
               model.UserName,
               model.Password,
               true,
#if DEBUG
               false
#else
                true
#endif
               );

            if (!login_result.Succeeded)
            {
                _logger.LogWarning($"Неверное имя пользователя:{model.UserName} или пароль:{model.Password}");
                return Unauthorized(new { message = "Неверное имя пользователя или пароль" });
            }

            string token = _tockenService.GenerateJwtToken(model.UserName);
            _logger.LogInformation($"Отпрака токена пользователю:{model.UserName} ");
            return Ok(new UserInfoViewModel(user.Id, token, user.UserName, user.NickName));
        }

        /// <summary>
        /// Registers a new user with the specified registration details.
        /// </summary>
        /// <param name="model">The registration view model containing user details.</param>
        /// <returns>A JWT token and user information if successful, otherwise an error message.</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation($"Регистрация пользователя {model.UserName}");

            UserEntity user = model.FromRegToEntity(InitData.DEFAULT_USER_PICTURE);

            IdentityResult registration_result = await _userManager.CreateAsync(user, model.Password);

            if (!registration_result.Succeeded)
            {
                _logger.LogWarning($"В процессе регистрации пользователя {model.UserName} возникли ошибки :( " +
                    $"{string.Join(",", registration_result.Errors.Select(e => e.Description))}");

                return BadRequest("Не удалось зарегестрировать пользователя");
            }

            _logger.LogInformation($"Пользователь {model.UserName} успешно зарегестрирован");

            await _userManager.AddToRoleAsync(user, UserStatus.User.ToString());
            _logger.LogInformation($"Пользователь {model.UserName} наделён ролью {UserStatus.User}");

            string token = _tockenService.GenerateJwtToken(model.UserName);
            _logger.LogInformation($"Отпрака токена пользователю:{model.UserName} ");
            return Ok(new UserInfoViewModel(user.Id, token, user.UserName, user.NickName));
        }

        /// <summary>
        /// Validates a given JWT token.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>True if the token is valid; otherwise, false.</returns>
        [AllowAnonymous]
        [HttpPost("checktoken")]
        public IActionResult CheckToken(string token)
        {
            bool isValid = _tockenService.ValidateToken(token);
            _logger.LogInformation($"Результат проверки токена: {token}: {isValid}");

            return Ok(isValid);
        }

        /// <summary>
        /// Retrieves user information based on the user ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve. Use -1 to get the authenticated user.</param>
        /// <returns>The user information if found, otherwise an error message.</returns>
        [HttpGet("getuser")]
        public async Task<IActionResult> GetUser(int id)
        {
            UserEntity? user;
            bool isAuth = false;

            if (User.Identity.IsAuthenticated)
            {
                UserEntity? authenticatedUser = await _userManager.FindByNameAsync(User.Identity.Name);
                if(authenticatedUser == null)
                {
                    _logger.LogWarning($"Аутенфицировнный пользователь userName:{User.Identity.Name} не найден");
                    return NotFound($"Аутенфицировнный пользователь userName:{User.Identity.Name} не найден");
                }

                if (id == -1)
                {
                    user = authenticatedUser;
                    isAuth = true;
                }
                else
                {
                    user = await _userManager.FindByIdAsync(id.ToString());
                    isAuth = user != null? user.Id == authenticatedUser.Id : false;
                }
            }
            else
            {
                user = await _userManager.FindByIdAsync(id.ToString());
            }

            if (user == null)
            {
                _logger.LogWarning($"Пользователь id:{id} не найден");
                return NotFound($"Пользователь id:{id} не найден");
            }

            return Ok(user.ToLongModel(isAuth, _env.WebRootPath));
        }

        /// <summary>
        /// Updates the user's profile image.
        /// </summary>
        /// <param name="image">The image file to upload.</param>
        /// <returns>The updated image information if successful, otherwise an error message.</returns>
        [Authorize]
        [HttpPatch("setimage")]
        public async Task<IActionResult> SetImage(IFormFile image)
        {
            UserEntity? user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь Name:{User.Identity!.Name!} не найден");
                return NotFound($"Пользователь Name:{User.Identity!.Name!} не найден");
            }

            ImageUploadResult SavingUserImageResult = await _imageManager.SaveUserImageAsync(image, user.UserName);
            if (!SavingUserImageResult.Success)
            {
                _logger.LogWarning($"Не получилось добавить изображение пользователю:{user.UserName} \n {SavingUserImageResult.ErrorMessage}");
                return BadRequest("Не удалось загрузить изображение.");
            }

            var updatedUserResult = await _userManager.UpdateImage(user.UserName, SavingUserImageResult.FilePath);

            if (!updatedUserResult.Succeeded)
            {
                _logger.LogWarning($"Не получилось обновить пользователя:{user.UserName}");
                return BadRequest("Не удалось обновить пользователя");
            }

            return Ok(new ImageViewModel(SavingUserImageResult.FilePath, _env.WebRootPath, $"image-{user.UserName}"));
        }

        /// <summary>
        /// Updates the user's nickname.
        /// </summary>
        /// <param name="nickName">The new nickname to set for the user.</param>
        /// <returns>True if the update is successful, otherwise an error message.</returns>
        [Authorize]
        [HttpPatch("setnickname")]
        public async Task<IActionResult> SetNickName(string nickName)
        {
            UserEntity? user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь Name:{User.Identity!.Name!} не найден");
                return NotFound($"Пользователь Name:{User.Identity!.Name!} не найден");
            }

            var result = await _userManager.UpdateNickName(user.UserName, nickName);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"Не удалось обновить пользователя {user.UserName}");
                return BadRequest("Не удалось обновить пользователя");
            }

            return Ok(true);
        }

        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <returns>A list of users with their IDs and usernames.</returns>
        [HttpGet("allref")]
        public IActionResult GetUsers() =>
            Ok(_userManager.Users.Select(us => new UserRefViewModel(us.Id, us.UserName)));

    }
}
