using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestForumServer.Database.Context;
using TestForumServer.Domian.Entities.ForumEntities.Contents;

namespace TestForumServer.Infrastructure.Services.Stores.Contents
{
    /// <summary>
    /// An abstract base class for managing CRUD operations on entities of type <typeparamref name="TEntity"/> in a database context.
    /// This class implements the <see cref="IStore{TEntity}"/> interface and provides common functionality
    /// for adding, updating, deleting, and retrieving entities, as well as handling likes.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that extends <see cref="ContentEntity"/>.</typeparam>
    public abstract class ContentStoreBase<TEntity> : IStore<TEntity> where TEntity : ContentEntity?
    {
        private readonly TestForumDbContext _dbContext;
        private readonly ILogger<ContentStoreBase<TEntity>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentStoreBase{TEntity}"/> class with the specified database context and logger.
        /// </summary>
        /// <param name="dbContext">The database context for managing entity persistence.</param>
        /// <param name="logger">The logger for logging operations.</param>
        protected ContentStoreBase(TestForumDbContext dbContext, ILogger<ContentStoreBase<TEntity>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public virtual async Task<TEntity> AddAsync(TEntity item)
        {
            _logger.LogInformation($"Начало работы метода {nameof(AddAsync)} класса {nameof(ContentStoreBase<TEntity>)}");
            ValidationEntityWithException(item, nameof(AddAsync), nameof(ContentStoreBase<TEntity>));

            TEntity? existingItem = CheckEntityExist(GetWithTracking(item.Id), nameof(AddAsync), nameof(ContentStoreBase<TEntity>));

            if (existingItem != null)
                return existingItem;

            EntityEntry<TEntity> result;
            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                result = _dbContext.Entry(item);
                result.State = EntityState.Added;

                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
            }
            _logger.LogInformation($"Объект {item.Id} успешно обнавлён в БД в методе {nameof(AddAsync)} класса {nameof(ContentStoreBase<TEntity>)}");
            return result.Entity;
        }

        /// <inheritdoc/>
        public virtual async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation($"Начало работы метода {nameof(DeleteAsync)} класса {nameof(ContentStoreBase<TEntity>)}");
            TEntity? existingItem = CheckEntityExist(GetWithTracking(id), nameof(DeleteAsync), nameof(ContentStoreBase<TEntity>));

            if (existingItem == null)
                return false;

            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                _dbContext.Entry(existingItem).State = EntityState.Deleted;

                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
            }
            _logger.LogInformation($"Объект id:{id} успешно удалён в БД в методе {nameof(DeleteAsync)} класса {nameof(ContentStoreBase<TEntity>)}");
            return true;
        }

        /// <inheritdoc/>
        public abstract TEntity? Get(int id);

        /// <summary>
        /// Retrieves an entity from the database by its identifier, with tracking enabled.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <returns>The requested entity, or <c>null</c> if not found.</returns>
        protected abstract TEntity? GetWithTracking(int id);

        /// <inheritdoc/>
        public abstract IQueryable<TEntity> GetAll();

        /// <summary>
        /// Retrieves all entities from the database, with tracking enabled.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> representing the collection of all entities.</returns>
        protected abstract IQueryable<TEntity> GetAllWithTracking();

        /// <inheritdoc/>
        public virtual async Task<TEntity?> UpdateAsync(TEntity item)
        {
            _logger.LogInformation($"Начало работы метода {nameof(UpdateAsync)} класса {nameof(ContentStoreBase<TEntity>)}");
            ValidationEntityWithException(item, nameof(UpdateAsync), nameof(ContentStoreBase<TEntity>));

            TEntity? existingItem = CheckEntityExist(Get(item.Id), nameof(UpdateAsync), nameof(ContentStoreBase<TEntity>));
            if (existingItem == null)
                return null;

            if (item.UserId != existingItem.UserId)
                throw new InvalidOperationException("Нельзя менять User");

            EntityEntry<TEntity> result;
            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                result = _dbContext.Entry(item);
                result.State = EntityState.Modified;
                result.Property(e => e.CreatedAt).IsModified = false;

                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
            }
            _logger.LogInformation($"Объект id:{item.Id} успешно обновлён в БД в методе {nameof(UpdateAsync)} класса {nameof(ContentStoreBase<TEntity>)}");
            return result.Entity;
        }

        /// <inheritdoc/>
        public async Task<TEntity?> UpdateImageAsync(int id, string newImage)
        {
            _logger.LogInformation($"Начало работы метода {nameof(UpdateImageAsync)} класса {nameof(ContentStoreBase<TEntity>)}");
            TEntity? existingItem = CheckEntityExist(GetWithTracking(id), nameof(UpdateImageAsync), nameof(ContentStoreBase<TEntity>));

            if (existingItem == null)
                return null;

            existingItem.ProfilePicturePath = newImage;

            return await UpdateAsync(existingItem);
        }

        /// <inheritdoc/>
        public async Task<TEntity?> UpdateContentAsync(int id, string content)
        {
            _logger.LogInformation($"Начало работы метода {nameof(UpdateContentAsync)} класса {nameof(ContentStoreBase<TEntity>)}");
            TEntity? existingItem = CheckEntityExist(GetWithTracking(id), nameof(UpdateContentAsync), nameof(ContentStoreBase<TEntity>));

            if (existingItem == null)
                return null;

            existingItem.Content = content;

            return await UpdateAsync(existingItem);
        }

        /// <inheritdoc/>
        public abstract Task<bool> LikeContentAsync(int id, int userId);

        /// <inheritdoc/>
        public abstract Task<bool> UnLikeContentAsync(int id, int userId);

        /// <inheritdoc/>
        public abstract Task<bool> ToggleLikeContentAsync(int id, int userId);

        /// <summary>
        /// Checks whether the specified entity exists in the database.
        /// </summary>
        /// <param name="existingItem">The entity to check.</param>
        /// <param name="methodName">The name of the method where this check is performed.</param>
        /// <param name="className">The name of the class where this check is performed.</param>
        /// <returns>The existing entity if found; otherwise, <c>null</c>.</returns>
        protected TEntity? CheckEntityExist(TEntity existingItem, string methodName, string className)
        {
            _logger.LogInformation($"Начало работы метода {nameof(CheckEntityExist)} класса {nameof(ContentStoreBase<TEntity>)}");

            if (existingItem == null)
                _logger.LogInformation($"Объект не существует в БД в методе {methodName} класса {className}");
            else
                _logger.LogInformation($"Объект id:{existingItem.Id} существует в БД в методе {methodName} класса {className}");
            return existingItem;
        }

        /// <summary>
        /// Validates the specified entity and throws an exception if it is invalid.
        /// </summary>
        /// <param name="item">The entity to validate.</param>
        /// <param name="methodName">The name of the method where this validation is performed.</param>
        /// <param name="className">The name of the class where this validation is performed.</param>
        /// <exception cref="ArgumentNullException">Thrown when the item is null or the user associated with the item is not found.</exception>
        protected void ValidationEntityWithException(TEntity item, string methodName, string className)
        {
            _logger.LogInformation($"Начало работы метода {nameof(ValidationEntityWithException)} класса {nameof(ContentStoreBase<TEntity>)}");
            if (item == null)
            {
                _logger.LogError($"Параметр {nameof(item)} равен {null} в методе {methodName} класса {className}");
                throw new ArgumentNullException("", $"Параметр item=null");
            }

            var user = _dbContext.Users.AsNoTracking().FirstOrDefault(el => el.Id == item.UserId);
            if (user == null)
            {
                _logger.LogError($"Параметр {nameof(user)} равен {null} в методе {methodName} класса {className}");
                throw new ArgumentNullException("", $"user == null");
            }
        }
    }
}
