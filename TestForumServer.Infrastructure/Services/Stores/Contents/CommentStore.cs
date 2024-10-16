using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestForumServer.Database.Context;
using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.Entities.ForumEntities.Likes;

namespace TestForumServer.Infrastructure.Services.Stores.Contents
{
    /// <summary>
    /// A class for managing comments in the database, inheriting from <see cref="ContentStoreBase{CommentEntity}"/>.
    /// This class provides methods for adding, updating, retrieving, and liking/unliking comments.
    /// </summary>
    public class CommentStore : ContentStoreBase<CommentEntity>
    {
        private readonly TestForumDbContext _dbContext;
        private readonly ILogger<CommentStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentStore"/> class with the specified database context and logger.
        /// </summary>
        /// <param name="dbContext">The database context for managing comment persistence.</param>
        /// <param name="logger">The logger for logging operations.</param>
        public CommentStore(TestForumDbContext dbContext, ILogger<CommentStore> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<CommentEntity> AddAsync(CommentEntity item)
        {
            ValidationPost(item, nameof(AddAsync));

            return await base.AddAsync(item);
        }

        /// <inheritdoc/>
        public override async Task<CommentEntity?> UpdateAsync(CommentEntity item)
        {
            ValidationPost(item, nameof(UpdateAsync));

            CommentEntity? existingItem = Get(item.Id);
            if (existingItem == null)
                return null;

            if (item.PostId != existingItem.PostId)
                throw new InvalidOperationException("Нельзя менять Post");

            return await base.UpdateAsync(item);
        }

        /// <inheritdoc/>
        public override CommentEntity? Get(int id) =>
            GetAll().FirstOrDefault(el => el.Id == id);

        /// <summary>
        /// Retrieves a comment from the database by its identifier, with tracking enabled.
        /// </summary>
        /// <param name="id">The identifier of the comment to retrieve.</param>
        /// <returns>The requested comment, or <c>null</c> if not found.</returns>
        protected override CommentEntity? GetWithTracking(int id) =>
             GetAllWithTracking().FirstOrDefault(el => el.Id == id);

        /// <inheritdoc/>
        public override IQueryable<CommentEntity> GetAll() =>
            GetAllWithTracking().AsNoTracking().OrderByDescending(el => el.CreatedAt);

        /// <summary>
        /// Retrieves all comments from the database, with tracking enabled.
        /// </summary>
        /// <returns>An <see cref="IQueryable{CommentEntity}"/> representing the collection of all comments.</returns>
        protected override IQueryable<CommentEntity> GetAllWithTracking() =>
            _dbContext.Comments;

        /// <inheritdoc/>
        public override async Task<bool> LikeContentAsync(int id, int userId)
        {
            _logger.LogInformation($"Начало работы метода {nameof(LikeContentAsync)} класса {nameof(CommentStore)}");
            CommentLikeEntity? existingCommentLike = _dbContext.CommentLikes.FirstOrDefault(el => el.CommentId == id && el.UserId == userId);
            if (existingCommentLike != null)
            {
                _logger.LogInformation($"commentlike CommentId:{existingCommentLike.CommentId} userid:{existingCommentLike.UserId} существует в БД " +
                    $"в методе {nameof(LikeContentAsync)} класса {nameof(CommentStore)}");
                return false;
            }

            CommentLikeEntity commentLike = new CommentLikeEntity
            {
                CommentId = id,
                UserId = userId
            };

            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                await _dbContext.CommentLikes.AddAsync(commentLike);
                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
            }
            _logger.LogInformation($"Лайк Comment id:{id} userid:{userId} успешно поставлен в БД в методе {nameof(LikeContentAsync)} класса {nameof(CommentStore)}");
            return true;
        }

        /// <inheritdoc/>
        public override async Task<bool> UnLikeContentAsync(int id, int userId)
        {
            _logger.LogInformation($"Начало работы метода {nameof(UnLikeContentAsync)} класса {nameof(CommentStore)}");
            CommentLikeEntity? existingCommentLike = _dbContext.CommentLikes.FirstOrDefault(el => el.CommentId == id && el.UserId == userId);
            if (existingCommentLike == null)
            {
                _logger.LogInformation($"commentLike commentId:{id} userid:{userId} не существует в БД " +
                    $"в методе {nameof(UnLikeContentAsync)} класса {nameof(CommentStore)}");
                return false;
            }

            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                _dbContext.CommentLikes.Remove(existingCommentLike);
                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
            }
            _logger.LogInformation($"Лайк comment id:{id} userid:{userId} успешно убран в БД в методе {nameof(DeleteAsync)} класса {nameof(CommentStore)}");
            return true;
        }

        /// <inheritdoc/>
        public override async Task<bool> ToggleLikeContentAsync(int id, int userId)
        {
            var like = _dbContext.CommentLikes.FirstOrDefault(l => l.CommentId == id && l.UserId == userId);

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
        /// Validates the specified comment and throws an exception if it is invalid.
        /// </summary>
        /// <param name="item">The comment to validate.</param>
        /// <param name="methodName">The name of the method where this validation is performed.</param>
        /// <exception cref="ArgumentNullException">Thrown when the post associated with the comment is not found.</exception>
        private void ValidationPost(CommentEntity item, string methodName)
        {
            _logger.LogInformation($"Начало работы метода {methodName} класса {nameof(CommentStore)}");
            ValidationEntityWithException(item, methodName, nameof(CommentStore));

            PostEntity? post = _dbContext.Posts.FirstOrDefault(p => p.Id == item.PostId);
            if (post == null)
                throw new ArgumentNullException("", $"Параметр post=null");
        }
    }
}
