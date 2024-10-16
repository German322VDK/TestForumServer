using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TestForumServer.Database.Context;
using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.Entities.ForumEntities.Likes;
using TestForumServer.Domian.Entities.Identity;
using TestForumServer.Infrastructure.StaticData;

namespace TestForumServer.Infrastructure.Initializers
{
    /// <summary>
    /// Initializes the database and Identity system for the Test Forum application.
    /// </summary>
    public class TestForumDbInitializer
    {
        private readonly ILogger<TestForumDbInitializer> _logger;
        private readonly TestForumDbContext _dbContext;
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<RoleEntity> _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestForumDbInitializer"/> class.
        /// </summary>
        /// <param name="dbContext">The database context for the Test Forum application.</param>
        /// <param name="userManager">The user manager for handling user operations.</param>
        /// <param name="roleManager">The role manager for handling role operations.</param>
        /// <param name="logger">The logger for logging information and errors.</param>
        public TestForumDbInitializer(
            TestForumDbContext dbContext,
            UserManager<UserEntity> userManager,
            RoleManager<RoleEntity> roleManager,
            ILogger<TestForumDbInitializer> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// Initializes the database, applying migrations and seeding data as necessary.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Initialize()
        {
            Stopwatch timer = Stopwatch.StartNew();

            using (_logger.BeginScope("Инициализация бд"))
            {
                _logger.LogInformation("Инициализация базы данных...");
            }

            DatabaseFacade db = _dbContext.Database;

            if (db.GetPendingMigrations().Any())
            {
                _logger.LogInformation("Выполнение миграций...");

                db.Migrate();

                _logger.LogInformation("Выполнение миграций выполнено успешно");
            }
            else
            {
                _logger.LogInformation($"База данных находится в актуальной версии ({timer.Elapsed.TotalSeconds:0.0###} c)");
            }

            try
            {
                await InitialIdentitiesAsync();
                await InitialForumAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Ошибка при выполнении инициализации БД :(");
                timer.Stop();
                throw;
            }

            _logger.LogInformation($"Инициализация БД выполнена успешно {timer.Elapsed.TotalSeconds}");
            timer.Stop();
        }

        /// <summary>
        /// Initializes the Identity system, creating roles and the main user if they do not exist.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task InitialIdentitiesAsync()
        {
            Stopwatch timer = Stopwatch.StartNew();

            _logger.LogInformation("Инициализация системы Identity...");

            if (await _userManager.FindByNameAsync(InitData.MAIN_USER_NAME) != null)
            {
                _logger.LogInformation("Инициализация БД Identity не требуется");
                return;
            }

            await CheckRole(UserStatus.Admin.ToString());
            await CheckRole(UserStatus.User.ToString());
            await CheckRole(UserStatus.Banned.ToString());

            _logger.LogInformation("Отсутствует учётная запись Главы");

            UserEntity mianUser = new UserEntity
            {
                NickName = "Главный",
                UserName = InitData.MAIN_USER_NAME,
                ProfilePicturePath = InitData.MAIN_USER_PICTURE,
            };

            IdentityResult creation_result = await _userManager.CreateAsync(mianUser, InitData.MAIN_USER_PASS);

            if (creation_result.Succeeded)
            {
                _logger.LogInformation("Учётная запись Главы создана успешно.");

                await _userManager.AddToRoleAsync(mianUser, UserStatus.Admin.ToString());

                _logger.LogInformation($"Учётная запись Главы наделена ролью {UserStatus.Admin.ToString()}");
            }
            else
            {
                IEnumerable<string> errors = creation_result.Errors.Select(e => e.Description);
                timer.Stop();
                throw new InvalidOperationException($"Ошибка при создании учётной записи " +
                    $"Главы:( ({string.Join(",", errors)})");
            }

            _logger.LogInformation($"Инициализация системы Identity завершена успешно за " +
                $"{timer.Elapsed.Seconds:0.0##}с");
            timer.Stop();
        }

        /// <summary>
        /// Initializes forum data, creating the main trad, post, and comment if they do not exist.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task InitialForumAsync()
        {
            if (_dbContext.Trads.Any())
            {
                _logger.LogInformation("Инициализация БД трэдом не требуется");
                return;
            }

            _logger.LogInformation("Инициализация трэдов...");

            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                UserEntity user = _dbContext.Users.OrderBy(el => el.Id).FirstOrDefault();

                TradEntity trad = new TradEntity
                {
                    Content = InitData.MAIN_TRAD_CONTENT,
                    UserId = user.Id,
                    ProfilePicturePath = InitData.MAIN_TRAD_PICTURE,
                };
                await _dbContext.Trads.AddAsync(trad);
                await _dbContext.SaveChangesAsync();

                PostEntity post = new PostEntity
                {
                    Content = InitData.MAIN_POST_CONTENT,
                    UserId = user.Id,
                    TradId = trad.Id,
                    ProfilePicturePath = InitData.MAIN_POST_PICTURE,
                };
                await _dbContext.Posts.AddAsync(post);
                await _dbContext.SaveChangesAsync();

                CommentEntity comment = new CommentEntity
                {
                    Content = InitData.MAIN_COMMENT_CONTENT,
                    PostId = post.Id,
                    UserId = user.Id,
                    ProfilePicturePath = InitData.MAIN_COMMENT_PICTURE,
                };
                await _dbContext.Comments.AddAsync(comment);
                await _dbContext.SaveChangesAsync();

                PostLikeEntity postLike = new PostLikeEntity { UserId = user.Id, PostId = post.Id };
                CommentLikeEntity commentLike = new CommentLikeEntity { UserId = user.Id, CommentId = comment.Id };
                TradLikeEntity tradLike = new TradLikeEntity { UserId = user.Id, TradId = trad.Id };

                await _dbContext.TradLikes.AddAsync(tradLike);
                await _dbContext.PostLikes.AddAsync(postLike);
                await _dbContext.CommentLikes.AddAsync(commentLike);
                await _dbContext.SaveChangesAsync();

                await _dbContext.Database.CommitTransactionAsync();
            }

            _logger.LogInformation("Инициализация трэдов выполнена успешно");
        }

        /// <summary>
        /// Checks if a role exists and creates it if it does not.
        /// </summary>
        /// <param name="RoleName">The name of the role to check.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task CheckRole(string RoleName)
        {
            if (!await _roleManager.RoleExistsAsync(RoleName))
            {
                _logger.LogInformation($"Роль {RoleName} отсуствует. Создаю...");

                await _roleManager.CreateAsync(new RoleEntity
                {
                    Name = RoleName,
                });

                _logger.LogInformation($"Роль {RoleName} создана успешно");
            }
        }
    }
}
