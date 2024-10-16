using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestForumServer.Database.Context;
using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.Entities.ForumEntities.Likes;

namespace TestForumServer.Infrastructure.Services.Stores.Contents
{
    /// <summary>
    /// A class for managing posts in the database, inheriting from <see cref="ContentStoreBase{PostEntity}"/>.
    /// This class provides methods for adding, updating, retrieving, and liking/unliking posts.
    /// </summary>
    public class PostStore : ContentStoreBase<PostEntity>
    {
        private readonly TestForumDbContext _dbContext;
        private readonly ILogger<PostStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostStore"/> class with the specified database context and logger.
        /// </summary>
        /// <param name="dbContext">The database context for managing post persistence.</param>
        /// <param name="logger">The logger for logging operations.</param>
        public PostStore(TestForumDbContext dbContext, ILogger<PostStore> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<PostEntity> AddAsync(PostEntity item)
        {
            ValidationTrad(item, nameof(AddAsync));

            return await base.AddAsync(item);
        }

        /// <inheritdoc/>
        public override async Task<PostEntity?> UpdateAsync(PostEntity item)
        {
            ValidationTrad(item, nameof(UpdateAsync));

            PostEntity? existingItem = Get(item.Id);
            if (existingItem == null)
                return null;

            if (item.TradId != existingItem.TradId)
                throw new InvalidOperationException("Нельзя менять Trad");

            return await base.UpdateAsync(item);
        }

        /// <inheritdoc/>
        public override PostEntity? Get(int id) =>
            GetAll().FirstOrDefault(el => el.Id == id);

        /// <summary>
        /// Retrieves a post from the database by its identifier, with tracking enabled.
        /// </summary>
        /// <param name="id">The identifier of the post to retrieve.</param>
        /// <returns>The requested post, or <c>null</c> if not found.</returns>
        protected override PostEntity? GetWithTracking(int id) =>
             GetAllWithTracking().FirstOrDefault(el => el.Id == id);

        /// <inheritdoc/>
        public override IQueryable<PostEntity> GetAll() =>
            GetAllWithTracking().AsNoTracking().OrderByDescending(el => el.CreatedAt);

        /// <summary>
        /// Retrieves all posts from the database, with tracking enabled.
        /// </summary>
        /// <returns>An <see cref="IQueryable{PostEntity}"/> representing the collection of all posts.</returns>
        protected override IQueryable<PostEntity> GetAllWithTracking() =>
            _dbContext.Posts;

        /// <inheritdoc/>
        public override async Task<bool> LikeContentAsync(int id, int userId)
        {
            _logger.LogInformation($"Начало работы метода {nameof(LikeContentAsync)} класса {nameof(PostStore)}");
            PostLikeEntity? existingPostLike = _dbContext.PostLikes.FirstOrDefault(el => el.PostId == id && el.UserId == userId);
            if (existingPostLike != null)
            {
                _logger.LogInformation($"postLike postId:{existingPostLike.PostId} userid:{existingPostLike.UserId} существует в БД " +
                    $"в методе {nameof(LikeContentAsync)} класса {nameof(PostStore)}");
                return false;
            }

            PostLikeEntity postLike = new PostLikeEntity
            {
                PostId = id,
                UserId = userId
            };

            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                await _dbContext.PostLikes.AddAsync(postLike);
                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
            }
            _logger.LogInformation($"Лайк Post id:{id} userid:{userId} успешно поставлен в БД в методе {nameof(DeleteAsync)} класса {nameof(PostStore)}");
            return true;
        }

        /// <inheritdoc/>
        public override async Task<bool> UnLikeContentAsync(int id, int userId)
        {
            _logger.LogInformation($"Начало работы метода {nameof(UnLikeContentAsync)} класса {nameof(PostStore)}");
            PostLikeEntity? existingPostLike = _dbContext.PostLikes.FirstOrDefault(el => el.PostId == id && el.UserId == userId);
            if (existingPostLike == null)
            {
                _logger.LogInformation($"postlike postId:{id} userid:{userId} не существует в БД " +
                    $"в методе {nameof(UnLikeContentAsync)} класса {nameof(PostStore)}");
                return false;
            }

            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                _dbContext.PostLikes.Remove(existingPostLike);
                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
            }
            _logger.LogInformation($"Лайк Post id:{id} userid:{userId} успешно убран в БД в методе {nameof(DeleteAsync)} класса {nameof(PostStore)}");
            return true;
        }

        /// <inheritdoc/>
        public override async Task<bool> ToggleLikeContentAsync(int id, int userId)
        {
            var like = _dbContext.PostLikes.FirstOrDefault(l => l.PostId == id && l.UserId == userId);

            if (like == null)
            {
                return await LikeContentAsync(id, userId);
            }
            else
            {
                return await UnLikeContentAsync(id, userId);
            }
        }

        /// <summary>
        /// Validates the specified post and throws an exception if it is invalid.
        /// </summary>
        /// <param name="item">The post to validate.</param>
        /// <param name="methodName">The name of the method where this validation is performed.</param>
        /// <exception cref="ArgumentNullException">Thrown when the trad associated with the post is not found.</exception>
        private void ValidationTrad(PostEntity item, string methodName)
        {
            _logger.LogInformation($"Начало работы метода {methodName} класса {nameof(PostStore)}");
            ValidationEntityWithException(item, methodName, nameof(PostStore));

            TradEntity? trad = _dbContext.Trads.FirstOrDefault(t => t.Id == item.TradId);
            if (trad == null)
                throw new ArgumentNullException("", $"Параметр trad=null");
        }
    }
}
