using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestForumServer.Database.Context;
using TestForumServer.Domian.Entities.ForumEntities.Contents;
using TestForumServer.Domian.Entities.ForumEntities.Likes;

namespace TestForumServer.Infrastructure.Services.Stores.Contents
{
    /// <summary>
    /// A class for managing trads in the database, inheriting from <see cref="ContentStoreBase{TradEntity}"/>.
    /// This class provides methods for retrieving, liking, and unliking trads.
    /// </summary>
    public class TradStore : ContentStoreBase<TradEntity>
    {
        private readonly TestForumDbContext _dbContext;
        private readonly ILogger<TradStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradStore"/> class with the specified database context and logger.
        /// </summary>
        /// <param name="dbContext">The database context for managing trad persistence.</param>
        /// <param name="logger">The logger for logging operations.</param>
        public TradStore(TestForumDbContext dbContext, ILogger<TradStore> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override TradEntity? Get(int id) =>
            GetAll().FirstOrDefault(el => el.Id == id);

        /// <summary>
        /// Retrieves a trad from the database by its identifier, with tracking enabled.
        /// </summary>
        /// <param name="id">The identifier of the trad to retrieve.</param>
        /// <returns>The requested trad, or <c>null</c> if not found.</returns>
        protected override TradEntity? GetWithTracking(int id) =>
             GetAllWithTracking().FirstOrDefault(el => el.Id == id);

        /// <inheritdoc/>
        public override IQueryable<TradEntity> GetAll() =>
             GetAllWithTracking().AsNoTracking().OrderByDescending(el => el.CreatedAt);

        /// <summary>
        /// Retrieves all trads from the database, with tracking enabled.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TradEntity}"/> representing the collection of all trads.</returns>
        protected override IQueryable<TradEntity> GetAllWithTracking() =>
            _dbContext.Trads;

        /// <inheritdoc/>
        public override async Task<bool> LikeContentAsync(int id, int userId)
        {
            _logger.LogInformation($"Начало работы метода {nameof(LikeContentAsync)} класса {nameof(TradStore)}");
            TradLikeEntity? existingTradLike = _dbContext.TradLikes.FirstOrDefault(el => el.TradId == id && el.UserId == userId);
            if (existingTradLike != null)
            {
                _logger.LogInformation($"tradlike tradid:{existingTradLike.TradId} userid:{existingTradLike.UserId} существует в БД " +
                    $"в методе {nameof(LikeContentAsync)} класса {nameof(TradStore)}");
                return false;
            }

            TradLikeEntity tradLike = new TradLikeEntity
            {
                TradId = id,
                UserId = userId
            };

            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                await _dbContext.TradLikes.AddAsync(tradLike);
                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
            }
            _logger.LogInformation($"Лайк Post id:{id} userid:{userId} успешно поставлен в БД в методе {nameof(DeleteAsync)} класса {nameof(TradStore)}");
            return true;
        }

        /// <inheritdoc/>
        public override async Task<bool> UnLikeContentAsync(int id, int userId)
        {
            _logger.LogInformation($"Начало работы метода {nameof(UnLikeContentAsync)} класса {nameof(TradStore)}");
            TradLikeEntity? existingTradLike = _dbContext.TradLikes.FirstOrDefault(el => el.TradId == id && el.UserId == userId);
            if (existingTradLike == null)
            {
                _logger.LogInformation($"tradlike tradid:{id} userid:{userId} не существует в БД " +
                    $"в методе {nameof(UnLikeContentAsync)} класса {nameof(TradStore)}");
                return false;
            }

            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                _dbContext.TradLikes.Remove(existingTradLike);
                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
            }
            _logger.LogInformation($"Лайк Post id:{id} userid:{userId} успешно убран в БД в методе {nameof(DeleteAsync)} класса {nameof(TradStore)}");
            return true;
        }

        /// <inheritdoc/>
        public override async Task<bool> ToggleLikeContentAsync(int id, int userId)
        {
            var like = _dbContext.TradLikes.FirstOrDefault(l => l.TradId == id && l.UserId == userId);

            if (like == null)
            {
                return await LikeContentAsync(id, userId);
            }
            else
            {
                return await UnLikeContentAsync(id, userId);
            }
        }
    }
}
